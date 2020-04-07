using System;
using System.Threading.Tasks;
using bledevice;
using bledevice.Microbit;
using bledevice.Microbit.Services;
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

            var microbit = await Microbit.ScanAndConnect(timeout: TimeSpan.FromSeconds(scanSeconds));

            Log.Information($"{microbit}");
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
    }
}