using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bledevice.PoweredUp.Helpers.DefaultableDictionary;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Voltage Sensor inside of all Powered Up Hubs
    /// </summary>
    public class VoltageSensor : LPF2Device, ISensor
    {
        private const byte MODE_VOLTAGE = 0x00;

        private static readonly IDictionary<LPF2HubType, double> MAX_VOLTAGE_VALUES =
            new Dictionary<LPF2HubType, double>()
            {
                {LPF2HubType.DUPLO_TRAIN_BASE, 6.4},
                {LPF2HubType.REMOTE_CONTROL, 6.4}
            }.WithDefaultValue(9.615);

        private static readonly IDictionary<LPF2HubType, int> MAX_VOLTAGE_RAW =
            new Dictionary<LPF2HubType, int>()
            {
                {LPF2HubType.DUPLO_TRAIN_BASE, 3047},
                {LPF2HubType.REMOTE_CONTROL, 3200},
                {LPF2HubType.TECHNIC_MEDIUM_HUB, 4095},
            }.WithDefaultValue(3893);


        public byte DefaultMode => MODE_VOLTAGE;
        public bool AutoSubscribe => true;

        internal VoltageSensor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.VOLTAGE_SENSOR)
        {
            Mode = MODE_VOLTAGE;
        }

        internal override async Task ReceiveMessage(Message msg)
        {
            if (OnVoltageChange != null)
            {
                var value = BitConverter.ToUInt16(msg.Payload.Slice(1));
                var maxValue = MAX_VOLTAGE_VALUES[Hub.HubType];
                var maxRaw = MAX_VOLTAGE_RAW[Hub.HubType];
                var current = ((double) value) * maxValue / maxRaw;
                await OnVoltageChange(this, current);
            }
        }

        /// <summary>
        /// Event handler for voltage change
        /// </summary>
        /// <param name="sender">The sensor which sends the event</param>
        /// <param name="voltage">Voltage value</param>
        public delegate Task VoltageHandler(VoltageSensor sender, double voltage);

        /// <summary>
        /// Event for voltage change
        /// </summary>
        public event VoltageHandler? OnVoltageChange;
    }
}