using System;
using System.Threading.Tasks;
using HashtagChris.DotNetBlueZ;
using Serilog;

namespace bledevice.GeneralServices
{
    public class BatteryService : GattService
    {
        private static readonly Guid BATTERY_LEVEL_UUID = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB");

#pragma warning disable CS8618
        public BatteryService(IGattService1 service) : base(service)
        {
        }

        public async Task<byte> GetBatteryLevel()
        {
            return await ReadUInt8(BATTERY_LEVEL_UUID);
        }

        public delegate Task BatteryEventHandlerAsync(byte level);

        private event BatteryEventHandlerAsync Handler;

        public async Task Subscribe(BatteryEventHandlerAsync handlerAsync)
        {
            if (Handler == null)
            {
                Handler += handlerAsync;
                await Subscribe(BATTERY_LEVEL_UUID, this.OnBatteryEvent);
            }
            else
            {
                Handler += handlerAsync;
            }
        }

#pragma warning disable CS1998
        public async Task Unsubscribe(BatteryEventHandlerAsync handlerAsync)
        {
            if (Handler != null)
            {
                Handler -= handlerAsync;
            }
        }

        private async Task OnBatteryEvent(GattCharacteristic sender,
            GattCharacteristicValueEventArgs eventArgs)
        {
            var p = eventArgs.Value[0];
            var g = Guid.Parse(await sender.GetUUIDAsync());
            if (g == BATTERY_LEVEL_UUID)
            {
                if (Handler != null) await Handler(p);
            }
            else
            {
                Log.Warning($"Unknown characteristic, UUID is {g}.");
            }
        }
    }
}