using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using HashtagChris.DotNetBlueZ;
using HashtagChris.DotNetBlueZ.Extensions;
using Serilog;

namespace bledevice
{
    [Serializable]
    public class BleDeviceError : Exception
    {
        public BleDeviceError(string message)
            : base(message)
        {
        }
    }

    public class ServiceNotFound : BleDeviceError
    {
        public ServiceNotFound(Guid svc) : base($"Service {svc} not found.")
        {
        }
    }

    public class CharacteristicNotFound : BleDeviceError
    {
        public CharacteristicNotFound(Guid ch) : base($"Characteristic {ch} not found.")
        {
        }
    }

    public class ScanFilter
    {
        private readonly string? _address;
        private readonly string? _name;

        public ScanFilter(string? address = null, string? name = null)
        {
            if (address == null && name == null)
            {
                throw new BleDeviceError("Invalid scan filter.");
            }

            _address = address;
            _name = name;
        }

        public async Task<bool> Check(IDevice1 device)
        {
            var deviceProperties = await device.GetAllAsync();
            if (_address == null)
            {
                return deviceProperties?.Alias?.Contains(_name ?? "") ?? false;
            }
            else
            {
                return deviceProperties.Address == _address;
            }
        }
    }

    public class BleDevice
    {
        private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(15);
        private static readonly Guid DEVICE_INFORMATION_SERVICE = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
        private static readonly Guid BATTERY_SERVICE = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");

        private readonly IDevice1 _device;
        private Device1Properties _properties;
        private readonly Dictionary<Guid, IGattService1> _services = new Dictionary<Guid, IGattService1>();

        public DeviceInformationService? DeviceInformationService { get; private set; }
        public BatteryService? BatteryService { get; private set; }

        protected static async Task<IDevice1> ScanAndConnectInternal(ScanFilter filter,
            TimeSpan? timeout = null,
            string? adapter = null)
        {
            // Default value
            if (timeout == null)
                timeout = TimeSpan.FromSeconds(10);
            IAdapter1? a;
            if (adapter == null)
            {
                var adapters = await BlueZManager.GetAdaptersAsync();
                if (adapters.Count == 0)
                {
                    Log.Error("No Bluetooth adapters found.");
                    throw new BleDeviceError("No Bluetooth adapters found.");
                }

                a = adapters.First();
            }
            else
            {
                a = await BlueZManager.GetAdapterAsync(adapter);
            }

            var adapterPath = a.ObjectPath.ToString();
            var adapterName = adapterPath.Substring(adapterPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
            Log.Debug($"Using Bluetooth Adapter {adapterName}.");

            var devices = await a.GetDevicesAsync();
            foreach (var device in devices)
            {
                var properties = await device.GetAllAsync();
                var deviceDescription = await GetDeviceDescriptionAsync(device, properties);
                Log.Debug(deviceDescription);
                if (await CheckAndConnect(filter, device))
                {
                    return device;
                }
            }

            Log.Debug($"{devices.Count} device(s) found ahead of scan.");

            // Scan for more devices.
            Log.Debug($"Scanning for {timeout.Value.Seconds} seconds...");

            IDevice1? device1 = null;
            var tokenSource = new CancellationTokenSource();
            using (await a.WatchDevicesAddedAsync(async device =>
            {
                var deviceProperties = await device.GetAllAsync();
                var deviceDescription = await GetDeviceDescriptionAsync(device, deviceProperties);
                Log.Debug($"[NEW] {deviceDescription}");
                if (!await CheckAndConnect(filter, device)) return;
                device1 = device;
                Log.Debug("Stopping scan...");
                tokenSource.Cancel();
            }))
            {
                Log.Debug("Starting scanning...");
                await a.StartDiscoveryAsync();
                await Task.Delay(TimeSpan.FromSeconds(timeout.Value.Seconds), tokenSource.Token);
                await a.StopDiscoveryAsync();
                Log.Debug("Scan complete.");
            }

            if (device1 != null)
            {
                return device1;
            }

            Log.Warning("Micro:bit not found.");
            throw new BleDeviceError("Device not found.");
        }

        public static async Task<BleDevice?> ScanAndConnect(ScanFilter filter,
            TimeSpan? timeout = null,
            string? adapter = null)
        {
            var dev = await ScanAndConnectInternal(filter, timeout, adapter);
            var ret = new BleDevice(dev);
            await ret.Initialize();
            return ret;
        }

        private static async Task<bool> ShouldConnect(ScanFilter filter, IDevice1 device)
        {
            var deviceProperties = await device.GetAllAsync();
            if (!await filter.Check(device)) return false;
            Log.Debug($"Device found at {deviceProperties.Address}.");
            return true;
        }

        private static async Task<bool> CheckAndConnect(ScanFilter filter, IDevice1 device)
        {
            if (!await ShouldConnect(filter, device)) return false;
            await device.ConnectAsync();
            await device.WaitForPropertyValueAsync("Connected", value: true, TIMEOUT);
            return true;
        }

        private static async Task<string> GetDeviceDescriptionAsync(IDevice1 device,
            Device1Properties? deviceProperties = null)
        {
            if (deviceProperties == null)
            {
                deviceProperties = await device.GetAllAsync();
            }

            return $"{deviceProperties.Alias} (Address: {deviceProperties.Address}, RSSI: {deviceProperties.RSSI})";
        }

#pragma warning disable CS8618
        protected BleDevice(IDevice1 device)
        {
            _device = device;
        }

        protected async Task Initialize()
        {
            await _device.WaitForPropertyValueAsync("Connected", value: true, TIMEOUT);
            _properties = await _device.GetAllAsync();
            Log.Debug("Waiting for services to resolve...");
            await _device.WaitForPropertyValueAsync("ServicesResolved", value: true, TIMEOUT);
            foreach (var s in await _device.GetUUIDsAsync())
            {
                var g = Guid.Parse(s);
                var svc = await _device.GetServiceAsync(s);
                _services[Guid.Parse(s)] = svc;
                if (g == DEVICE_INFORMATION_SERVICE)
                {
                    Log.Debug("Found DeviceInformationService.");
                    DeviceInformationService = new DeviceInformationService(svc);
                }
                else if (g == BATTERY_SERVICE)
                {
                    Log.Debug("Found BatteryService.");
                    BatteryService = new BatteryService(svc);
                }
            }

            Log.Debug("Services resolved.");

            await BleDeviceConnected();
            Log.Information($"Connected to {this}.");
        }

#pragma warning disable CS1998
        protected virtual async Task BleDeviceConnected()
        {
        }

        public async Task Disconnect()
        {
            await _device.DisconnectAsync();
            await _device.WaitForPropertyValueAsync("Connected", value: false, TIMEOUT);
            Log.Debug($"{this} disconnected.");
        }

        public bool HasService(Guid svc)
        {
            return _services.ContainsKey(svc);
        }

        public bool HasDeviceInformationService()
        {
            return DeviceInformationService != null;
        }

        public bool HasBatteryService()
        {
            return BatteryService != null;
        }

        public override string ToString()
        {
            try
            {
                return $"BleDevice(alias=\"{_properties.Alias}\", address=\"{_properties.Address}\")";
            }
            catch (NullReferenceException)
            {
                return "BleDevice(unknown)";
            }
        }

        protected IGattService1? GetService(Guid svc)
        {
            return _services.ContainsKey(svc) ? _services[svc] : null;
        }
    }
}