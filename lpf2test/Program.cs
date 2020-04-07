using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Devices;
using bledevice.PowerUp.Hubs;
using Serilog;

namespace lpf2test
{
    class Program
    {
        static async Task Main(string[] args)
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
            motor.BrakingStyle = TachoMotor.BrakingStyles.HOLD;
            await motor.SetSpeed(70, 30, TimeSpan.FromSeconds(3));
            // await motor.SetSpeed(70, 30);
            //await motor.SetSpeed(50);
            // await motor.SetPower(50);
            // await Task.Delay(TimeSpan.FromSeconds(3));
            // await motor.SetPower(0);
            await Task.Delay(TimeSpan.FromMinutes(10));
            // await movehub.Disconnect();
        }
    }
}