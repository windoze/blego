using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using bledevice.PoweredUp.Devices;
using bledevice.PoweredUp.Protocol;
using HashtagChris.DotNetBlueZ;
using Serilog;

// ReSharper disable InconsistentNaming
namespace bledevice.PoweredUp.Hubs
{
    public class LPF2Error : BleDeviceError
    {
        public LPF2Error(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Generic LPF2 Hub
    /// </summary>
    public abstract class LPF2Hub : BleDevice
    {
        #region Properties

        /// <summary>
        /// Hub Type
        /// </summary>
        public LPF2HubType HubType { get; protected set; } = LPF2HubType.UNKNOWN;

        /// <summary>
        /// All available port ids, include virtual ports 
        /// </summary>
        public IEnumerable<int> Ports =>
            from n in PortIdMap.Zip(Enumerable.Range(0, byte.MaxValue))
            where n.First != null
            select n.Second;

        /// <summary>
        /// All port names, include virtual ports, the name of the virtual port is like "A+B"
        /// </summary>
        public IEnumerable<string> PortNames => from name in PortIdMap where name != null select name;

        /// <summary>
        /// Hub Firmware Version
        /// </summary>
        public string FirmwareVersion { get; private set; } = "";

        /// <summary>
        /// Hub Hardware Version
        /// </summary>
        public string HardwareVersion { get; private set; } = "";

        /// <summary>
        /// Signal Strength
        /// </summary>
        public int RSSI { get; private set; } = 0;

        /// <summary>
        /// Bluetooth MAC Address
        /// </summary>
        public string PrimaryMACAddress { get; private set; } = "";

        /// <summary>
        /// Battery Voltage
        /// </summary>
        public int BatteryVoltage { get; private set; } = 0;

        #endregion

        #region Events

        /// <summary>
        /// Event for BLE connection established for the Hub 
        /// </summary>
        /// <param name="sender">The Hub</param>
        public delegate Task ConnectHandler(LPF2Hub sender);

        /// <summary>
        /// Event for BLE connection disconnected for the Hub 
        /// </summary>
        /// <param name="sender">The Hub</param>
        public delegate Task DisconnectHandler(LPF2Hub sender);

        /// <summary>
        /// Event for Button State Change 
        /// </summary>
        /// <param name="sender">The Hub</param>
        /// <param name="state">The state of the button on the Hub</param>
        public delegate Task ButtonStateHandler(LPF2Hub sender, ButtonState state);

        /// <summary>
        /// Event for Signal Strength Change 
        /// </summary>
        /// <param name="sender">The Hub</param>
        /// <param name="rssi">Received signal strength</param>
        public delegate Task RSSIHandler(LPF2Hub sender, int rssi);

        /// <summary>
        /// Event for Battery Voltage Change 
        /// </summary>
        /// <param name="sender">The Hub</param>
        /// <param name="batteryVoltage">Battery Voltage</param>
        public delegate Task BatteryVoltageHandler(LPF2Hub sender, int batteryVoltage);

        /// <summary>
        /// Event for LPF2 device attachment
        /// </summary>
        /// <param name="sender">The Hub</param>
        /// <param name="device">The attached device</param>
        /// <param name="portId">The port id</param>
        public delegate Task DeviceAttachHandler(LPF2Hub sender, LPF2Device? device, int portId);

        /// <summary>
        /// Event for LPF2 device detachment
        /// </summary>
        /// <param name="sender">The Hub</param>
        /// <param name="device">The detached device</param>
        /// <param name="portId">The port id</param>
        public delegate Task DeviceDetachHandler(LPF2Hub sender, LPF2Device? device, int portId);

        /// <summary>
        /// Connect event handler
        /// </summary>
        public event ConnectHandler? OnConnect;

        /// <summary>
        /// Disconnect event handler
        /// </summary>
        public event DisconnectHandler? OnDisconnect;

        /// <summary>
        /// Button event handler
        /// </summary>
        public event ButtonStateHandler? OnButtonStateChange;

        /// <summary>
        /// RSSI event handler
        /// </summary>
        public event RSSIHandler? OnRSSIChange;

        /// <summary>
        /// Battery voltage event handler
        /// </summary>
        public event BatteryVoltageHandler? OnBatteryVoltageChange;

        /// <summary>
        /// Device attachment event handler
        /// </summary>
        public event DeviceAttachHandler? OnDeviceAttach;

        /// <summary>
        /// Device detachment event handler
        /// </summary>
        public event DeviceDetachHandler? OnDeviceDetach;

        #endregion

        #region Public Methods

        /// <summary>
        /// Create virtual port
        /// </summary>
        /// <param name="port1">The name of the first port</param>
        /// <param name="port2">The name of the second port</param>
        /// <returns></returns>
        public async Task CreateVirtualPort(string port1, string port2)
        {
            int portId1 = GetPortId(port1) ?? throw new LPF2Error($"Port {port1} doesn't have device attached.");
            int portId2 = GetPortId(port2) ?? throw new LPF2Error($"Port {port2} doesn't have device attached.");
            await Send(Message.MessageType.VirtualPortSetup, new byte[] {0x01, (byte) portId1, (byte) portId2});
        }

        /// <summary>
        /// Get Port Name
        /// </summary>
        /// <param name="portId">The port id</param>
        /// <returns>The port name, null if the port doesn't exist</returns>
        public string? GetPortName(int portId)
        {
            return PortIdMap[portId];
        }

        /// <summary>
        /// Get Port Id
        /// </summary>
        /// <param name="Name">The Port Name</param>
        /// <returns>The port id, null if the port doesn't exist</returns>
        public int? GetPortId(string Name)
        {
            var x = from p in Ports where PortIdMap[p] == Name select p;
            var enumerable = x as int[] ?? x.ToArray();
            if (!enumerable.Any()) return null;
            return enumerable.First();
        }

        /// <summary>
        /// Get the device attached to the port
        /// </summary>
        /// <param name="portId">the port id</param>
        /// <returns>The device, null if the port doesn't exist or has no device attached</returns>
        public LPF2Device? GetDevice(int portId)
        {
            return _devices[portId];
        }

        /// <summary>
        /// Get the device attached to the port
        /// </summary>
        /// <param name="portName">the port name</param>
        /// <returns>The device, null if the port doesn't exist or has no device attached</returns>
        public LPF2Device? GetDevice(string portName)
        {
            return _devices[Array.IndexOf(PortIdMap, portName)];
        }

        /// <summary>
        /// Find the first device with given type
        /// </summary>
        /// <param name="type">The device type</param>
        /// <returns>The device, null if there is no such type of device attached</returns>
        public LPF2Device? FindFirstDevice(LPF2DeviceType type)
        {
            return _devices.DefaultIfEmpty(null).FirstOrDefault((lpf2Device => lpf2Device?.Type == type));
        }

        /// <summary>
        /// Find all devices with given type
        /// </summary>
        /// <param name="type">The device type</param>
        /// <returns>The list of found devices, could be empty if there is no such type of device attached</returns>
        public IEnumerable<LPF2Device> FindAllDevices(LPF2DeviceType type)
        {
            return from dev in _devices where dev?.Type == type select dev;
        }

        #endregion

        #region Implementations

        private class LegoHubService : GattService
        {
            public LegoHubService(IGattService1 service) : base(service)
            {
            }
        }

        private LegoHubService Service => _service.Value;

        protected readonly string?[] PortIdMap = new string[byte.MaxValue];
        private readonly Dictionary<byte, string> VirtualPortMap = new Dictionary<byte, string>();

        // LPF2 Hubs have only one service
        private static readonly Guid LEGO_HUB_SERVICE = Guid.Parse("00001623-1212-efde-1623-785feabcd123");

        // LPF2 Hubs have only one characteristic
        private static readonly Guid LEGO_HUB_CHARACTERISTIC = Guid.Parse("00001624-1212-efde-1623-785feabcd123");

        // ReSharper disable once InconsistentNaming
        private Lazy<LegoHubService> _service => new Lazy<LegoHubService>(() =>
        {
            var svc = GetService(LEGO_HUB_SERVICE);
            if (svc == null) throw new LPF2Error("LEGO Hub Service Not Found.");
            return new LegoHubService(svc);
        });

        // Max port id seems to be 255?
        protected readonly LPF2Device?[] _devices = new LPF2Device[byte.MaxValue];

        private delegate Task MessageHandler(Message msg);

        private readonly MessageHandler?[] _callbacks = new MessageHandler[
            (int) Enum.GetValues(typeof(Message.HubPropertyType)).Cast<Message.HubPropertyType>().Max()];

        private static string FormatVersion(int ver)
        {
            var s = $"{ver:X8}";
            var parts = new[] {s[0].ToString(), s[1].ToString(), s.Substring(2, 2), s.Substring(4)};
            return string.Join('.', parts);
        }

        private static string FormatMACAddress(Span<byte> data)
        {
            return string.Join(':', from b in data.ToArray() select $"{b:X2}");
        }

        protected LPF2Hub(IDevice1 device) : base(device)
        {
        }

        protected override async Task BleDeviceConnected()
        {
            await Service.Subscribe(LEGO_HUB_CHARACTERISTIC, this.OnHubEvent);
            await Task.Delay(500);
            // Enable Button Notification
            await EnablePropertyNotification(Message.HubPropertyType.Button);
            // Get Firmware Version
            await UpdateHubPropertyValue(Message.HubPropertyType.FirmwareVersion);
            // Get Hardware Version
            await UpdateHubPropertyValue(Message.HubPropertyType.HardwareVersion);
            // Enable RSSI Notification
            await EnablePropertyNotification(Message.HubPropertyType.RSSI);
            // Enable Battery Voltage Notification
            await EnablePropertyNotification(Message.HubPropertyType.BatteryVoltage);
            // Get Primary MAC Address
            await UpdateHubPropertyValue(Message.HubPropertyType.PrimaryMACAddress);
            await SetupHub();
            if (OnConnect != null)
            {
                await OnConnect(this);
            }
        }

        protected abstract Task SetupHub();

        internal async Task Send(Message.MessageType type, byte[] payload)
        {
            await Service.WriteCharacteristic(LEGO_HUB_CHARACTERISTIC, new Message(type, payload).ToByteArray());
        }

        private async Task OnMessage(Message msg)
        {
            // TODO:
        }

        private async Task EnablePropertyNotification(Message.HubPropertyType propertyType)
        {
            await Send(Message.MessageType.HubProperties,
                new[] {(byte) propertyType, (byte) Message.HubPropertyOperation.EnableUpdates});
        }

        private async Task UpdateHubPropertyValue(Message.HubPropertyType propertyType)
        {
            var ret = new TaskCompletionSource<bool>();
            // Set one-off callback for this request
            _callbacks[(int) propertyType] = async msg =>
            {
                await OnHubProperties(msg);
                ret.SetResult(true);
            };
            await Send(Message.MessageType.HubProperties,
                new[] {(byte) propertyType, (byte) Message.HubPropertyOperation.RequestUpdate});
            await ret.Task;
        }

        private async Task OnHubProperties(Message msg)
        {
            switch (msg.PropertyType)
            {
                // case Message.HubPropertyType.AdvertisingName:
                //     break;
                case Message.HubPropertyType.Button:
                    if (OnButtonStateChange != null)
                    {
                        Log.Debug($"ButtonState: {msg.Payload[2]}.");
                        if (msg.Payload[2] == 0)
                        {
                            await OnButtonStateChange(this, ButtonState.RELEASED);
                        }
                        else if (msg.Payload[2] == 2)
                        {
                            await OnButtonStateChange(this, ButtonState.PRESSED);
                        }
                    }

                    break;
                case Message.HubPropertyType.FirmwareVersion:
                    FirmwareVersion = FormatVersion(BitConverter.ToInt32(msg.ToByteArray(), 5));
                    break;
                case Message.HubPropertyType.HardwareVersion:
                    HardwareVersion = FormatVersion(BitConverter.ToInt32(msg.ToByteArray(), 5));
                    break;
                case Message.HubPropertyType.RSSI:
                    RSSI = msg.Payload[2];
                    OnRSSIChange?.Invoke(this, RSSI);
                    break;
                case Message.HubPropertyType.BatteryVoltage:
                    BatteryVoltage = msg.Payload[2];
                    OnBatteryVoltageChange?.Invoke(this, BatteryVoltage);
                    break;
                // case Message.HubPropertyType.BatteryType:
                //     break;
                // case Message.HubPropertyType.ManufactureName:
                //     break;
                // case Message.HubPropertyType.RadioFirmwareVersion:
                //     break;
                // case Message.HubPropertyType.LEGOWirelessProtocolVersion:
                //     break;
                // case Message.HubPropertyType.SystemTypeID:
                //     break;
                // case Message.HubPropertyType.HardwareNetworkID:
                //     break;
                case Message.HubPropertyType.PrimaryMACAddress:
                    PrimaryMACAddress = FormatMACAddress(new Span<byte>(msg.ToByteArray(), 5, 6));
                    break;
                // case Message.HubPropertyType.SecondaryMACAddress:
                //     break;
                // case Message.HubPropertyType.HardwareNetworkFamily:
                //     break;
            }
        }

        private async Task AttachDevice(LPF2Device? device, int portId)
        {
            if (device == null)
            {
                Log.Warning($"Attempt to attach null device to port {portId}.");
                return;
            }

            Log.Debug($"Attaching {device} to port {portId} on {this}.");
            device.IsVirtualPort = VirtualPortMap.ContainsKey((byte) portId);

            await device.Initialize();
            _devices[portId] = device;
            if (OnDeviceAttach != null)
            {
                await OnDeviceAttach(this, device, portId);
            }

            if (device is ISensor sensor)
            {
                if (sensor.AutoSubscribe)
                {
                    await EnablePortValueNotification(portId, sensor.DefaultMode);
                }
            }

            Log.Debug("Device attached.");
        }

        private async Task DetachDevice(int portId)
        {
            Log.Debug($"Detaching {_devices} from port {portId} on {this}.");
            var device = _devices[portId];
            if (device == null)
            {
                Log.Warning($"Device on port {portId} already detached.");
                return;
            }

            if (device is ISensor sensor)
            {
                if (sensor.AutoSubscribe)
                {
                    await DisablePortValueNotification(portId, sensor.DefaultMode);
                }
            }

            _devices[portId] = null;
            if (OnDeviceDetach != null)
            {
                await OnDeviceDetach(this, device, portId);
            }

            Log.Debug("Device detached.");
        }

        internal async Task EnablePortValueNotification(int portId, byte mode)
        {
            await Send(Message.MessageType.PortInputFormatSetupSingle,
                new byte[] {(byte) portId, mode, 0x01, 0x00, 0x00, 0x00, 0x01});
        }

        internal async Task DisablePortValueNotification(int portId, byte mode)
        {
            await Send(Message.MessageType.PortInputFormatSetupSingle,
                new byte[] {(byte) portId, mode, 0x01, 0x00, 0x00, 0x00, 0x00});
        }

        private async Task OnAttachedIo(Message msg)
        {
            var portId = msg.Payload[0];
            var eventType = msg.Payload[1];
            var deviceType = (eventType != 0) ? BitConverter.ToUInt16(msg.Payload.Slice(2)) : 0;
            switch (eventType)
            {
                case 0:
                    // Device detachment
                    await DetachDevice(portId);
                    if (VirtualPortMap.ContainsKey(portId))
                    {
                        // Remove virtual port
                        var name = VirtualPortMap[portId];
                        VirtualPortMap.Remove(portId);
                        PortIdMap[portId] = null;
                        Log.Debug($"VirtualPort(name={name}, id={portId}) removed.");
                    }

                    break;
                case 1:
                    // Device attachment
                    await AttachDevice(LPF2Device.CreateInstance(this, (LPF2DeviceType) deviceType, portId), portId);

                    break;
                case 2:
                    // Virtual port creation
                    var firstPortName = PortIdMap[msg.Payload[4]];
                    var secondPortName = PortIdMap[msg.Payload[5]];
                    var virtualPortName = $"{firstPortName}+{secondPortName}";
                    var virtualPortId = msg.Payload[0];
                    PortIdMap[virtualPortId] = virtualPortName;
                    VirtualPortMap[virtualPortId] = virtualPortName;
                    Log.Debug($"VirtualPort(name={virtualPortName}, id={virtualPortId}) created.");
                    await AttachDevice(LPF2Device.CreateInstance(this, (LPF2DeviceType) deviceType, virtualPortId),
                        virtualPortId);
                    break;
                default:
                    Log.Warning($"Unknown attached IO event {eventType}.");
                    break;
            }
        }

        private async Task UpdatePortInformation(int portId)
        {
            await Send(Message.MessageType.PortInformationRequest, new byte[] {(byte) portId, 0x01});
            await Send(Message.MessageType.PortInformationRequest, new byte[] {(byte) portId, 0x02});
        }

        private async Task OnPortInformation(Message msg)
        {
            // TODO:
        }

        private async Task UpdatePortModeInformation(int portId, byte mode, byte type)
        {
            await Send(Message.MessageType.PortModeInformationRequest, new byte[] {(byte) portId, mode, type});
        }

        private async Task OnPortModeInformation(Message msg)
        {
            // TODO:
        }

        private async Task OnSensorMessage(Message msg)
        {
            var portId = msg.Payload[0];
            var device = _devices[portId];
            if (device != null)
            {
                await device.ReceiveMessage(msg);
            }
        }

        private async Task OnPortAction(Message msg)
        {
            var portId = msg.Payload[0];
            var finished = msg.Payload[1];
            var device = _devices[portId];

            if (device != null && finished != 0)
            {
                await device.LastActionFinished();
            }
        }

        private async Task OnHubEvent(GattCharacteristic sender, GattCharacteristicValueEventArgs eventArgs)
        {
            var msg = new Message(eventArgs.Value);
            switch (msg.Type)
            {
                case Message.MessageType.HubProperties:
                    if (_callbacks[(int) msg.PropertyType] != null)
                    {
                        // NOTE: Callbacks are one-off, set by previous request operations
                        // LEGO protocol sucks, we cannot just read values, we have to write a request and wait for the next notification.
                        // Because every LOGO LPF2 Hub has only one service and one characteristic, so they decided to multiplex this only channel. 
                        var cb = _callbacks[(int) msg.PropertyType];
                        _callbacks[(int) msg.PropertyType] = null;
                        if (cb != null)
                        {
                            await cb(msg);
                        }
                    }
                    else
                    {
                        await OnHubProperties(msg);
                    }

                    break;
                // case Message.MessageType.HubActions:
                //     break;
                // case Message.MessageType.HubAlerts:
                //     break;
                case Message.MessageType.HubAttachedIO:
                    await OnAttachedIo(msg);
                    break;
                // case Message.MessageType.GenericErrorMessage:
                //     break;
                // case Message.MessageType.HardwareNetworkCommands:
                //     break;
                // case Message.MessageType.FirmwareUpdateGoIntoBootMode:
                //     break;
                // case Message.MessageType.FirmwareUpdateLockMemory:
                //     break;
                // case Message.MessageType.FirmwareUpdateLockStatusReport:
                //     break;
                // case Message.MessageType.FirmwareLockStatus:
                //     break;
                // case Message.MessageType.PortInformationRequest:
                //     break;
                // case Message.MessageType.PortModeInformationRequest:
                //     break;
                // case Message.MessageType.PortInputFormatSetupSingle:
                //     break;
                // case Message.MessageType.PortInputFormatSetupCombined:
                //     break;
                case Message.MessageType.PortInformation:
                    await OnPortInformation(msg);
                    break;
                case Message.MessageType.PortModeInformation:
                    await OnPortModeInformation(msg);
                    break;
                case Message.MessageType.PortValueSingle:
                    await OnSensorMessage(msg);
                    break;
                // case Message.MessageType.PortValueCombined:
                //     break;
                // case Message.MessageType.PortInputFormatSingle:
                //     break;
                // case Message.MessageType.PortInputFormatCombined:
                //     break;
                // case Message.MessageType.VirtualPortSetup:
                //     break;
                // case Message.MessageType.PortOutputCommand:
                //     break;
                case Message.MessageType.PortOutputCommandFeedback:
                    await OnPortAction(msg);
                    break;
            }

            await OnMessage(msg);
        }

        private static async Task<LPF2Hub?> CreateHubInstance(IDevice1 dev)
        {
            var data = await dev.GetManufacturerDataAsync();
            if (!data.ContainsKey(0x0397))
            {
                // Not a LEGO product
                return null;
            }

            var typeId = ((byte[]) data[0x0397])[1];
            LPF2Hub? ret = null;
            switch (typeId)
            {
                case 32: // Duplo Train Base
                    // TODO:
                    break;
                case 64: // LEGO Boost MoveHub
                    ret = new MoveHub(dev);
                    break;
                case 65: // Powered Up Hub
                    // TODO:
                    break;
                case 66: // Remote Controller
                    // TODO:
                    break;
                case 128: // Technic Medium Hub
                    ret = new TechnicMediumHub(dev);
                    break;
            }

            if (ret == null)
            {
                // Unknown LEGO Hub
                return null;
            }

            await ret.Initialize();
            return ret;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Scan and connect to the LPF2 Hub with given name
        /// </summary>
        /// <param name="name">The name of the hub</param>
        /// <param name="timeout">Time period the scanning process lasts, default value is 10 seconds.</param>
        /// <returns>The Hub instance, null if no hub found or connected</returns>
        public static async Task<LPF2Hub?> ScanAndConnect(string name, TimeSpan? timeout = null)
        {
            var dev = await ScanAndConnectInternal(new ScanFilter(name: name), timeout);
            return await CreateHubInstance(dev);
        }

        /// <summary>
        /// Connect to the LPF2 Hub at given MAC address.
        /// </summary>
        /// <param name="address">The Hub MAC address</param>
        /// <param name="timeout">Time period the scanning process lasts, default value is 10 seconds.</param>
        /// <returns></returns>
        /// <exception cref="LPF2Error">Thrown when the hub cannot be connected</exception>
        public static async Task<LPF2Hub> Connect(string address, TimeSpan? timeout = null)
        {
            var dev = await ScanAndConnectInternal(new ScanFilter(address: address), timeout);
            if (dev == null) throw new LPF2Error($"Device not found at address {address}");
            var ret = await CreateHubInstance(dev);
            if (ret == null) throw new LPF2Error($"Device at address {address} is not a supported LPF2 Hub.");
            return ret;
        }

        #endregion
    }
}