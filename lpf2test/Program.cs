using System;
using System.Linq;
using System.Threading.Tasks;
using bledevice.PoweredUp.Devices;
using bledevice.PoweredUp.Hubs;
using Serilog;

namespace lpf2test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await TestTechnicHub();
        }
        
        static async Task TestTechnicHub()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            //var movehub = await MoveHub.ScanAndConnect(TimeSpan.FromSeconds(10));
            // var movehub = await MoveHub.Connect("00:16:53:AE:33:59");
            if (!((await LPF2Hub.Connect("90:84:2B:4E:8E:43")) is TechnicMediumHub techhub))
            {
                Log.Error("MoveHub not found.");
                return;
            }

            Log.Information($"Firmware Version: {techhub.FirmwareVersion}");
            Log.Information($"Hardware Version: {techhub.HardwareVersion}");
            Log.Information($"RSSI: {techhub.RSSI}");
            Log.Information($"Battery Voltage: {techhub.BatteryVoltage}");
            Log.Information($"Primary MAC Address: {techhub.PrimaryMACAddress}");

            await techhub.LED.SetRGB(100, 0, 0);
            await Task.Delay(TimeSpan.FromSeconds(3));
            await techhub.LED.SetColor(Color.PINK);

            //movehub.TiltSensor.OnTilt += async (sender, x, y) => Log.Information($"x: {x}, y: {y}.");
            techhub.OnButtonStateChange += async (sender, state) => Log.Information($"ButtonState: {state}");

            techhub.OnDeviceAttach += async (sender, device, portId) =>
            {
                if (device is ColorDistanceSensor cds)
                {
                    Log.Information($"ColorDistanceSensor {cds} attached, setting up event handler...");
                    cds.OnColorChange += async (sensor, color) =>
                    {
                        Log.Information($"Setting color to {color}...");
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        await techhub.LED.SetColor(color);
                    };
                    cds.OnDistanceChange += async (sensor, distance) =>
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                        Log.Information($"Distance is {distance}...");
                    };
                } else if (device is Light light)
                {
                    foreach (var n in Enumerable.Range(1,9))
                    {
                        await light.SetBrightness((byte)(n * 10));
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    await light.SetBrightness(100);
                }
            };
            var motor = techhub.GetDevice("D") as MediumLinearMotor;
            //var motor = techhub.MotorAB;
            // await motor.SetSpeed(50, TimeSpan.FromSeconds(3));
            // await Task.Delay(TimeSpan.FromSeconds(2));
            // motor.BrakingStyle = TachoMotor.BrakingStyles.HOLD;
            // await motor.SetSpeed(70, 30, TimeSpan.FromSeconds(3));
            // await motor.SetSpeed(70, 30);
            //await motor.SetSpeed(50);
            await motor.SetPower(50);
            await Task.Delay(TimeSpan.FromSeconds(3));
            await motor.SetPower(0);
            await Task.Delay(TimeSpan.FromMinutes(10));
            // await techhub.Disconnect();
        }


        static async Task TestMoveHub()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            //var movehub = await MoveHub.ScanAndConnect(TimeSpan.FromSeconds(10));
            // var movehub = await MoveHub.Connect("00:16:53:AE:33:59");
            if (!((await LPF2Hub.Connect("00:16:53:AE:33:59")) is MoveHub movehub))
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
                if (device is ColorDistanceSensor cds)
                {
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
                }
            };
            // var motor = movehub.GetDevice("D") as MediumLinearMotor;
            var motor = movehub.MotorAB;
            // await motor.SetSpeed(50, TimeSpan.FromSeconds(3));
            // await Task.Delay(TimeSpan.FromSeconds(2));
            // motor.BrakingStyle = TachoMotor.BrakingStyles.HOLD;
            // await motor.SetSpeed(70, 30, TimeSpan.FromSeconds(3));
            // await motor.SetSpeed(70, 30);
            //await motor.SetSpeed(50);
            // await motor.SetPower(50);
            // await Task.Delay(TimeSpan.FromSeconds(3));
            // await motor.SetPower(0);
            // await Task.Delay(TimeSpan.FromMinutes(10));
            await movehub.Disconnect();
        }
    }
}