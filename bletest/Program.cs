using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using bledevice;
using bledevice.Microbit;
using bledevice.Microbit.Services;
using bledevice.PoweredUp.Devices;
using bledevice.PoweredUp.Hubs;
using Nito.AsyncEx;
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
                !int.TryParse(args[0], out var scanSeconds))
            {
                Console.WriteLine("Usage: scan <SecondsToScan> [adapterName]");
                Console.WriteLine("Example: scan 15 hci0");
                return;
            }

            // await Task.WhenAll(
            //     TestMicrobit(scanSeconds),
            //     new AsyncContextThread().Factory.Run(() => TestTechnicHub(scanSeconds)),
            //     new AsyncContextThread().Factory.Run(() => TestMoveHub(scanSeconds))
            // );
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

            await microbit.ButtonService.Subscribe(
                (button, pressed) =>
                {
                    var b = button == ButtonService.Button.A ? "Button A" : "Button B";
                    Log.Information(pressed ? $"{b} pressed." : $"{b} released.");
                    return Task.CompletedTask;
                });
            await microbit.AccelerometerService.WriteAccelerometerPeriod(1000);
            await microbit.AccelerometerService.Subscribe(
                (x, y, z) =>
                {
                    Log.Information($"x: {x}, y: {y}, z: {z}.");
                    return Task.CompletedTask;
                });

            await Blinky(microbit);

            await microbit.Disconnect();
        }

        private static async Task Blinky(Microbit microbit)
        {
            MatrixState[] glyphs =
            {
                MatrixState.HEART_FULL,
                MatrixState.HEART_EMPTY,
                MatrixState.CIRCLE_FULL,
                MatrixState.CIRCLE_EMPTY,
                MatrixState.SQUARE_FULL,
                MatrixState.SQUARE_EMPTY,
                MatrixState.DIAMOND_FULL,
                MatrixState.DIAMOND_EMPTY,
                MatrixState.X_MARK,
                MatrixState.CHECK_MARK,
                MatrixState.ARROW_UP,
                MatrixState.ARROW_UPRIGHT,
                MatrixState.ARROW_RIGHT,
                MatrixState.ARROW_DOWNRIGHT,
                MatrixState.ARROW_DOWN,
                MatrixState.ARROW_DOWNLEFT,
                MatrixState.ARROW_LEFT,
                MatrixState.ARROW_UPLEFT,
            };

            for (var i = 0; i < 50; i++)
            {
                await microbit.LedService.WriteLedMatrixState(glyphs[i % glyphs.Length]);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public static async Task TestLPF2Device(LPF2Device device)
        {
            if (device == null) return;
            switch (device)
            {
                case ColorDistanceSensor cds:
                    Log.Information($"ColorDistanceSensor {cds} attached, setting up event handler...");
                    cds.OnColorChange += async (sensor, color) =>
                    {
                        Log.Information($"Setting color to {color}...");
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        await device.Hub.LED.SetColor(color);
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
                    Log.Information($"Testing {device}...");
                    await tachoMotor.SetPower(50);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    await tachoMotor.Stop();
                    Log.Information("Done.");
                    break;
            }
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

            technichub.OnDeviceAttach += async (sender, device, portId) => { await TestLPF2Device(device); };

            var t1 = technichub.FindFirstDevice(LPF2DeviceType.TECHNIC_LARGE_LINEAR_MOTOR) as TachoMotor;
            var t2 = technichub.FindFirstDevice(LPF2DeviceType.TECHNIC_XLARGE_LINEAR_MOTOR) as TachoMotor;
            if (t1 != null) await t1.SetPower(50);
            if (t2 != null) await t2.SetPower(50);
            await Task.Delay(TimeSpan.FromSeconds(5));
            if (t1 != null) await t1.Stop();
            if (t2 != null) await t2.Stop();

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

            movehub.OnDeviceAttach += async (sender, device, portId) => { await TestLPF2Device(device); };

            var x = from port in movehub.Ports select TestLPF2Device(movehub.GetDevice(port));
            await Task.WhenAll(x.ToArray());

            var motor = movehub.MotorAB;
            await motor.SetSpeed(50, TimeSpan.FromSeconds(3));
            await Task.Delay(TimeSpan.FromSeconds(60));
            await movehub.Disconnect();
        }
    }
}