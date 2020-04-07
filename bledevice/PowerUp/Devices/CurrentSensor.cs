using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bledevice.PowerUp.Helpers.DefaultableDictionary;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;

namespace bledevice.PowerUp.Devices
{
    public class CurrentSensor : LPF2Device, ISensor
    {
        private const byte MODE_CURRENT = 0x00;

        private readonly IDictionary<LPF2HubType, int> MAX_CURRENT_VALUES = new Dictionary<LPF2HubType, int>()
        {
            {LPF2HubType.TECHNIC_MEDIUM_HUB, 4175}
        }.WithDefaultValue(2444);

        private readonly IDictionary<LPF2HubType, int> MAX_CURRENT_RAW =
            new Dictionary<LPF2HubType, int>().WithDefaultValue(4095);

        public byte DefaultMode => MODE_CURRENT;
        public bool AutoSubscribe => true;

        internal CurrentSensor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.CURRENT_SENSOR)
        {
        }

        internal override async Task ReceiveMessage(Message msg)
        {
            if (OnCurrentChange != null)
            {
                var value = BitConverter.ToUInt16(msg.Payload.Slice(1));
                // TODO: Convert Raw value base on Hub Type then call 
                var maxValue = MAX_CURRENT_VALUES[Hub.HubType];
                var maxRaw = MAX_CURRENT_RAW[Hub.HubType];
                var current = ((double) value) * maxValue / maxRaw;
                await OnCurrentChange(this, current);
            }
        }

        public delegate Task CurrentHandler(CurrentSensor sender, double current);

        public event CurrentHandler? OnCurrentChange;
    }
}