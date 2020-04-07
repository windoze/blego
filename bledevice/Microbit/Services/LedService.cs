using System;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using HashtagChris.DotNetBlueZ;

namespace bledevice.Microbit.Services
{
    public class LedService : GattService
    {
        private static readonly Guid LED_MATRIX_STATE_CHARACTERISTIC_UUID =
            Guid.Parse("e95d7b77251d470aa062fa1922dfa9a8");

        private static readonly Guid LED_TEXT_CHARACTERISTIC_UUID = Guid.Parse("e95d93ee251d470aa062fa1922dfa9a8");

        private static readonly Guid LED_SCROLLING_DELAY_CHARACTERISTIC_UUID =
            Guid.Parse("e95d0d2d251d470aa062fa1922dfa9a8");

        public LedService(IGattService1 service) : base(service)
        {
        }

        public async Task<byte[]> ReadLedMatrixState()
        {
            return await ReadCharacteristic(LED_MATRIX_STATE_CHARACTERISTIC_UUID);
        }

        public async Task WriteLedMatrixState(byte[] data)
        {
            await WriteCharacteristic(LED_MATRIX_STATE_CHARACTERISTIC_UUID, data);
        }

        public async Task WriteLedText(string data)
        {
            await Write(LED_TEXT_CHARACTERISTIC_UUID, data);
        }

        public async Task<ushort> ReadLedScrollingDelay()
        {
            return await ReadUInt16(LED_SCROLLING_DELAY_CHARACTERISTIC_UUID);
        }

        public async Task WriteLedScrollingDelay(ushort data)
        {
            await Write(LED_SCROLLING_DELAY_CHARACTERISTIC_UUID, data);
        }
    }
}