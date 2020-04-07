using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HashtagChris.DotNetBlueZ;
using HashtagChris.DotNetBlueZ.Extensions;
using Serilog;

namespace bledevice.GeneralServices
{
    public class GattService : IDisposable
    {
        private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(15);
        private readonly IGattService1 _service;

        private readonly ConcurrentDictionary<Guid, GattCharacteristic> _characteristics =
            new ConcurrentDictionary<Guid, GattCharacteristic>();

        private readonly Mutex _guard = new Mutex();

        public GattService(IGattService1 service)
        {
            _service = service;
        }

        private async Task<GattCharacteristic> GetCharacteristic(Guid ch)
        {
            if (!_characteristics.ContainsKey(ch))
            {
                _characteristics[ch] = (await _service.GetCharacteristicAsync(ch.ToString())) ??
                                       throw new CharacteristicNotFound(ch);
            }

            return _characteristics[ch];
        }

        public async Task<byte[]> ReadCharacteristic(Guid ch)
        {
            return await (await GetCharacteristic(ch)).ReadValueAsync(TIMEOUT);
        }

        public async Task WriteCharacteristic(Guid ch, byte[] data)
        {
            var c = await GetCharacteristic(ch);
            if (c == null)
            {
                Log.Warning($"Writing to non-existent characteristic {ch.ToString()}.");
                return;
            }

            var options = new Dictionary<string, object>();
            var writeTask = c.WriteValueAsync(data, options);
            var timeoutTask = Task.Delay(TIMEOUT);

            await Task.WhenAny(new Task[] {writeTask, timeoutTask});
            if (!writeTask.IsCompleted)
            {
                throw new TimeoutException("Timed out waiting to write characteristic value.");
            }

            await writeTask;
        }

        public async Task<string> ReadString(Guid ch)
        {
            return Encoding.UTF8.GetString(await ReadCharacteristic(ch));
        }

        public async Task<byte> ReadUInt8(Guid ch)
        {
            return (await ReadCharacteristic(ch))[0];
        }

        public async Task<sbyte> ReadInt8(Guid ch)
        {
            return unchecked((sbyte) (await ReadCharacteristic(ch))[0]);
        }

        public async Task<ushort> ReadUInt16(Guid ch)
        {
            return BitConverter.ToUInt16(await ReadCharacteristic(ch));
        }

        public async Task<short> ReadInt16(Guid ch)
        {
            return BitConverter.ToInt16(await ReadCharacteristic(ch));
        }

        public async Task<uint> ReadUInt32(Guid ch)
        {
            return BitConverter.ToUInt32(await ReadCharacteristic(ch));
        }

        public async Task<int> ReadInt32(Guid ch)
        {
            return BitConverter.ToInt32(await ReadCharacteristic(ch));
        }

        public async Task<float> ReadFloat(Guid ch)
        {
            return BitConverter.ToSingle(await ReadCharacteristic(ch));
        }

        public async Task<double> ReadDouble(Guid ch)
        {
            return BitConverter.ToDouble(await ReadCharacteristic(ch));
        }

        public async Task Write(Guid ch, byte data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, sbyte data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, ushort data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, short data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, uint data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, int data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, float data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, double data)
        {
            await WriteCharacteristic(ch, BitConverter.GetBytes(data));
        }

        public async Task Write(Guid ch, string data)
        {
            await WriteCharacteristic(ch, Encoding.UTF8.GetBytes(data));
        }

        public async Task Subscribe(Guid ch, GattCharacteristicEventHandlerAsync handler)
        {
            var c = await GetCharacteristic(ch);
            c.Value += handler;
        }

        public async Task Unsubscribe(Guid ch, GattCharacteristicEventHandlerAsync handler)
        {
            var c = await GetCharacteristic(ch);
            c.Value -= handler;
        }

        public void Dispose()
        {
            foreach (var characteristicsValue in _characteristics.Values)
            {
                characteristicsValue.Dispose();
            }

            _guard.Dispose();
        }
    }
}