using System;
using System.Threading.Tasks;
using bledevice.Microbit.Services;
using HashtagChris.DotNetBlueZ;

// ReSharper disable InconsistentNaming
namespace bledevice.Microbit
{
    public class Microbit : BleDevice
    {
        private static readonly Guid BUTTON_SERVICE = Guid.Parse("e95d9882251d470aa062fa1922dfa9a8");
        private static readonly Guid LED_SERVICE = Guid.Parse("e95dd91d251d470aa062fa1922dfa9a8");
        private static readonly Guid IO_PIN_SERVICE = Guid.Parse("e95d127b251d470aa062fa1922dfa9a8");
        private static readonly Guid ACCELEROMETER_SERVICE = Guid.Parse("e95d0753251d470aa062fa1922dfa9a8");

        private Lazy<ButtonService?> _ButtonService => new Lazy<ButtonService?>(() =>
        {
            var svc = GetService(BUTTON_SERVICE);
            return (svc != null) ? new ButtonService(svc) : null;
        });

        public ButtonService? ButtonService => _ButtonService.Value;

        private Lazy<LedService?> _LedService => new Lazy<LedService?>(() =>
        {
            var svc = GetService(LED_SERVICE);
            return (svc != null) ? new LedService(svc) : null;
        });

        public LedService? LedService => _LedService.Value;

        private Lazy<IoPinService?> _IoPinService => new Lazy<IoPinService?>(() =>
        {
            var svc = GetService(IO_PIN_SERVICE);
            return (svc != null) ? new IoPinService(svc) : null;
        });

        public IoPinService? IoPinService => _IoPinService.Value;

        private Lazy<AccelerometerService?> _AccelerometerService => new Lazy<AccelerometerService?>(() =>
        {
            var svc = GetService(ACCELEROMETER_SERVICE);
            return (svc != null) ? new AccelerometerService(svc) : null;
        });

        public AccelerometerService? AccelerometerService => _AccelerometerService.Value;

        private Microbit(IDevice1 device) : base(device)
        {
        }

        public static async Task<Microbit?> ScanAndConnect(string? address = null, TimeSpan? timeout = null)
        {
            var dev = await ScanAndConnectInternal(new ScanFilter(address: address, name: "BBC micro:bit"),
                timeout);
            if (dev == null) return null;
            var ret = new Microbit(dev);
            await ret.Initialize();
            return ret;
        }
    }
}