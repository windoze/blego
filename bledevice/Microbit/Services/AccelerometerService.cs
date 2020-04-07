using System;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using HashtagChris.DotNetBlueZ;

namespace bledevice.Microbit.Services
{
    public class AccelerometerService : GattService
    {
        private static readonly Guid ACCELEROMETER_CHARACTERISTIC_UUID =
            Guid.Parse("e95dca4b251d470aa062fa1922dfa9a8");

        private static readonly Guid ACCELEROMETER_PERIOD_CHARACTERISTIC_UUID =
            Guid.Parse("e95dfb24251d470aa062fa1922dfa9a8");

#pragma warning disable CS8618
        public AccelerometerService(IGattService1 service) : base(service)
        {
        }

        public async Task<Tuple<double, double, double>> ReadAccelerometer()
        {
            var data = await ReadCharacteristic(ACCELEROMETER_CHARACTERISTIC_UUID);
            var x = BitConverter.ToInt16(data) / 1000.0;
            var y = BitConverter.ToInt16(data, 2) / 1000.0;
            var z = BitConverter.ToInt16(data, 4) / 1000.0;
            return new Tuple<double, double, double>(x, y, z);
        }

        public async Task<ushort> ReadAccelerometerPeriod()
        {
            return await ReadUInt16(ACCELEROMETER_PERIOD_CHARACTERISTIC_UUID);
        }

        public async Task WriteAccelerometerPeriod(ushort period)
        {
            if (period >= 640)
            {
                period = 640;
            }
            else if (period >= 160)
            {
                period = 160;
            }
            else if (period >= 80)
            {
                period = 80;
            }
            else if (period >= 20)
            {
                period = 20;
            }
            else if (period >= 10)
            {
                period = 10;
            }
            else if (period >= 5)
            {
                period = 5;
            }
            else if (period >= 2)
            {
                period = 2;
            }
            else
            {
                period = 1;
            }

            await Write(ACCELEROMETER_PERIOD_CHARACTERISTIC_UUID, period);
        }

        public delegate Task AccelerometerEventHandlerAsync(double x, double y, double z);

        private event AccelerometerEventHandlerAsync Handler;

        public async Task Subscribe(AccelerometerEventHandlerAsync handlerAsync)
        {
            if (Handler == null)
            {
                Handler += handlerAsync;
                await Subscribe(ACCELEROMETER_CHARACTERISTIC_UUID, this.OnAccelerometerEvent);
            }
            else
            {
                Handler += handlerAsync;
            }
        }

#pragma warning disable CS1998
        public async Task Unsubscribe(AccelerometerEventHandlerAsync handlerAsync)
        {
            if (Handler != null)
            {
                Handler -= handlerAsync;
            }
        }

        private async Task OnAccelerometerEvent(GattCharacteristic sender,
            GattCharacteristicValueEventArgs eventArgs)
        {
            if (Handler != null)
            {
                var x = BitConverter.ToInt16(eventArgs.Value) / 1000.0;
                var y = BitConverter.ToInt16(eventArgs.Value, 2) / 1000.0;
                var z = BitConverter.ToInt16(eventArgs.Value, 4) / 1000.0;
                await Handler(x, y, z);
            }
        }
    }
}