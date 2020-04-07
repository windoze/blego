using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using HashtagChris.DotNetBlueZ;

namespace bledevice.Microbit.Services
{
    public class IoPinService : GattService
    {
        private static readonly Guid PIN_DATA_CHARACTERISTIC_UUID =
            Guid.Parse("e95d8d00251d470aa062fa1922dfa9a8");

        private static readonly Guid PIN_AD_CONFIGURATION_CHARACTERISTIC_UUID =
            Guid.Parse("e95d8d00251d470aa062fa1922dfa9a8");

        private static readonly Guid PIN_IO_CONFIGURATION_CHARACTERISTIC_UUID =
            Guid.Parse("e95d8d00251d470aa062fa1922dfa9a8");

        private uint _adMask = 0;
        private uint _ioMask = 0;

#pragma warning disable CS8618
        public IoPinService(IGattService1 service) : base(service)
        {
        }

        private async Task<Dictionary<byte, byte>> ReadPinData()
        {
            var ret = new Dictionary<byte, byte>();
            var data = await ReadCharacteristic(PIN_DATA_CHARACTERISTIC_UUID);
            for (var i = 0; i < data.Length; i += 2)
            {
                ret[data[i]] = ret[data[i + 1]];
            }

            return ret;
        }

        private async Task WritePinData(Dictionary<byte, byte> data)
        {
            var d = new byte[data.Count * 2];
            var idx = 0;
            foreach (var (k, v)in data)
            {
                d[idx] = k;
                d[idx + 1] = v;
                idx += 2;
            }

            await WriteCharacteristic(PIN_DATA_CHARACTERISTIC_UUID, d);
        }

        private async Task<uint> ReadPinAdConfiguration()
        {
            _adMask = await ReadUInt32(PIN_AD_CONFIGURATION_CHARACTERISTIC_UUID);
            return _adMask;
        }

        private async Task WritePinAdConfiguration(uint data)
        {
            await Write(PIN_AD_CONFIGURATION_CHARACTERISTIC_UUID, data);
            _adMask = data;
        }

        private async Task<uint> ReadPinIoConfiguration()
        {
            _ioMask = await ReadUInt32(PIN_IO_CONFIGURATION_CHARACTERISTIC_UUID);
            return _ioMask;
        }

        private async Task WritePinIoConfiguration(uint data)
        {
            await Write(PIN_IO_CONFIGURATION_CHARACTERISTIC_UUID, data);
            _ioMask = data;
        }

        public async Task SetPinAnalog(byte pin)
        {
            _adMask |= (uint) (1 << pin);
            await WritePinAdConfiguration(_adMask);
        }

        public async Task SetPinDigital(byte pin)
        {
            _adMask &= (uint) ~(1 << pin);
            await WritePinAdConfiguration(_adMask);
        }

        public async Task SetPinInput(byte pin)
        {
            _ioMask |= (uint) (1 << pin);
            await WritePinIoConfiguration(_adMask);
        }

        public async Task SetPinOutput(byte pin)
        {
            _ioMask &= (uint) ~(1 << pin);
            await WritePinIoConfiguration(_adMask);
        }

        public async Task<byte?> ReadPin(byte pin)
        {
            var data = await ReadPinData();
            if (data.ContainsKey(pin))
            {
                return data[pin];
            }

            return null;
        }

        public async Task WritePin(byte pin, byte value)
        {
            await WritePinData(new Dictionary<byte, byte> {{pin, value}});
        }

        public delegate Task PinEventHandlerAsync(byte pin, byte value);

        private event PinEventHandlerAsync Handler;

        public async Task Subscribe(PinEventHandlerAsync handlerAsync)
        {
            if (Handler == null)
            {
                Handler += handlerAsync;
                await Subscribe(PIN_DATA_CHARACTERISTIC_UUID, this.OnPinEvent);
            }
            else
            {
                Handler += handlerAsync;
            }
        }

#pragma warning disable CS1998
        public async Task Unsubscribe(PinEventHandlerAsync handlerAsync)
        {
            if (Handler != null)
            {
                Handler -= handlerAsync;
            }
        }

        private async Task OnPinEvent(GattCharacteristic sender,
            GattCharacteristicValueEventArgs eventArgs)
        {
            for (var i = 0; i < eventArgs.Value.Length; i += 2)
            {
                var pin = eventArgs.Value[i];
                var data = eventArgs.Value[i + 1];
                if (Handler != null)
                {
                    await Handler(pin, data);
                }
            }
        }
    }
}