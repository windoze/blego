using System;
using System.Linq;
using System.Threading.Tasks;
using bledevice;
using bledevice.Microbit;
using bledevice.Microbit.Services;
using bledevice.PoweredUp.Devices;
using bledevice.PoweredUp.Hubs;
using Serilog;

namespace bletest
{
    public static class Program
    {
        private static async Task SkipIfFail(Func<Task> f)
        {
            try
            {
                await f();
            }
            catch (CharacteristicNotFound)
            {
            }
        }

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            if (args.Length < 1 || args.Length > 2 || args[0].ToLowerInvariant() == "-h" ||
                !int.TryParse(args[0], out int scanSeconds))
            {
                Console.WriteLine("Usage: scan <SecondsToScan> [adapterName]");
                Console.WriteLine("Example: scan 15 hci0");
                return;
            }

            await Task.WhenAll(
                TestMicrobit(scanSeconds),
                TestTechnicHub(scanSeconds),
                TestMoveHub(scanSeconds)
            );
        }

        private static async Task TestMicrobit(int scanSeconds)
        {
            var microbit = await Microbit.ScanAndConnect(timeout: TimeSpan.FromSeconds(scanSeconds));
            if (microbit == null)
            {
                Log.Error("BBC Micro:bit not found.");
                return;
            }

            if (microbit.HasDeviceInformationService())
            {
                await SkipIfFail(async () =>
                    Log.Information($"System ID: {await microbit.DeviceInformationService.GetSystemId()}"));
                await SkipIfFail(async () =>
                    Log.Information($"Model Number: {await microbit.DeviceInformationService.GetModelNumber()}"));
                await SkipIfFail(async () =>
                    Log.Information($"Serial Number: {await microbit.DeviceInformationService.GetSerialNumber()}"));
                await SkipIfFail(async () =>
                    Log.Information(
                        $"Firmware Revision: {await microbit.DeviceInformationService.GetFirmwareRevision()}"));
                await SkipIfFail(async () =>
                    Log.Information(
                        $"Hardware Revision: {await microbit.DeviceInformationService.GetHardwareRevision()}"));
                await SkipIfFail(async () =>
                    Log.Information(
                        $"Software Revision: {await microbit.DeviceInformationService.GetSoftwareRevision()}"));
                await SkipIfFail(async () =>
                    Log.Information(
                        $"Manufacturer Name: {await microbit.DeviceInformationService.GetManufacturerName()}"));
            }
            else
            {
                Log.Information($"{microbit} doesn't have DeviceInformationService.");
            }

            if (microbit.HasBatteryService())
            {
                await SkipIfFail(async () =>
                    Log.Information($"Battery Level: {await microbit.BatteryService.GetBatteryLevel()}"));
            }
            else
            {
                Log.Information($"{microbit} doesn't have BatteryService.");
            }

            await microbit.LedService.WriteLedMatrixState(new byte[] {10, 21, 17, 10, 4});
            await microbit.ButtonService.Subscribe(
                (button, pressed) =>
                {
                    var b = button == ButtonService.Button.A ? "Button A" : "Button B";
                    Log.Information(pressed ? $"{b} pressed." : $"{b} released.");
                    return Task.CompletedTask;
                });
            await microbit.AccelerometerService.WriteAccelerometerPeriod(80);
            await microbit.AccelerometerService.Subscribe(
                (x, y, z) =>
                {
                    Log.Information($"x: {x}, y: {y}, z: {z}.");
                    return Task.CompletedTask;
                });
            await Task.Delay(TimeSpan.FromSeconds(60));
            await microbit.Disconnect();
        }

        private static async Task TestTechnicHub(int scanSeconds)
        {
            var technichub = await TechnicMediumHub.ScanAndConnect(timeout: TimeSpan.FromSeconds(scanSeconds));
            if (technichub == null)
            {
                Log.Error("Technic Medium Hub not found.");
                return;
            }

            Log.Information($"Firmware Version: {technichub.FirmwareVersion}");
            Log.Information($"Hardware Version: {technichub.HardwareVersion}");
            Log.Information($"RSSI: {technichub.RSSI}");
            Log.Information($"Battery Voltage: {technichub.BatteryVoltage}");
            Log.Information($"Primary MAC Address: {technichub.PrimaryMACAddress}");

            await technichub.LED.SetRGB(100, 0, 0);
            await Task.Delay(TimeSpan.FromSeconds(3));
            await technichub.LED.SetColor(Color.PINK);

            technichub.OnButtonStateChange += async (sender, state) => Log.Information($"ButtonState: {state}");

            technichub.OnDeviceAttach += async (sender, device, portId) =>
            {
                if (device is ColorDistanceSensor cds)
                {
                    Log.Information($"ColorDistanceSensor {cds} attached, setting up event handler...");
                    cds.OnColorChange += async (sensor, color) =>
                    {
                        Log.Information($"Setting color to {color}...");
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        await technichub.LED.SetColor(color);
                    };
                    cds.OnDistanceChange += async (sensor, distance) =>
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        Log.Information($"Distance is {distance}...");
                    };
                }
                else if (device is Light light)
                {
                    foreach (var n in Enumerable.Range(1, 9))
                    {
                        await light.SetBrightness((byte) (n * 10));
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }

                    await light.SetBrightness(100);
                }
                else if (device is TachoMotor motor)
                {
                    await motor.SetPower(50);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    await motor.SetPower(0);
                }
            };
            await Task.Delay(TimeSpan.FromSeconds(60));
            await technichub.Disconnect();
        }

        private static async Task TestMoveHub(int scanSeconds)
        {
            var movehub = await MoveHub.ScanAndConnect(TimeSpan.FromSeconds(scanSeconds));
            if (movehub == null)
            {
                Log.Error("MoveHub not found.");
                return;
            }

            Log.Information($"Firmware Version: {movehub.FirmwareVersion}");
            Log.Information($"Hardware Version: {movehub.HardwareVersion}");
            Log.Information($"RSSI: {movehub.RSSI}");
            Log.Information($"Battery Voltage: {movehub.BatteryVoltage}");
            Log.Information($"Primary MAC Address: {movehub.PrimaryMACAddress}");

            await movehub.LED.SetRGB(100, 0, 0);
            await Task.Delay(TimeSpan.FromSeconds(3));
            await movehub.LED.SetColor(Color.PINK);

            movehub.TiltSensor.OnTilt += async (sender, x, y) => Log.Information($"x: {x}, y: {y}.");
            movehub.OnButtonStateChange += async (sender, state) => Log.Information($"ButtonState: {state}");

            movehub.OnDeviceAttach += async (sender, device, portId) =>
            {
                switch (device)
                {
                    case ColorDistanceSensor cds:
                        Log.Information($"ColorDistanceSensor {cds} attached, setting up event handler...");
                        cds.OnColorChange += async (sensor, color) =>
                        {
                            Log.Information($"Setting color to {color}...");
                            await Task.Delay(TimeSpan.FromMilliseconds(1000));
                            await movehub.LED.SetColor(color);
                        };
                        cds.OnDistanceChange += async (sensor, distance) =>
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(1000));
                            Log.Information($"Distance is {distance}...");
                        };
                        break;
                    case Light light:
                    {
                        foreach (var n in Enumerable.Range(1, 9))
                        {
                            await light.SetBrightness((byte) (n * 10));
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }

                        await light.SetBrightness(100);
                        break;
                    }
                    case TachoMotor tachoMotor:
                        await tachoMotor.SetPower(50);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        await tachoMotor.SetPower(0);
                        break;
                }
            };
            var motor = movehub.MotorAB;
            await motor.SetSpeed(50, TimeSpan.FromSeconds(3));
            await Task.Delay(TimeSpan.FromSeconds(60));
            await movehub.Disconnect();
        }
    }
}