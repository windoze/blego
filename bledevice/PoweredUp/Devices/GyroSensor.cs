using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Gyro Sensor
    /// </summary>
    public class GyroSensor : LPF2Device, ISensor
    {
        private const byte MODE_GYRO = 0x00;

        internal GyroSensor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
            Mode = MODE_GYRO;
        }

        public byte DefaultMode => MODE_GYRO;
        public bool AutoSubscribe => true;

        /// <summary>
        /// Event handler for Gyro events
        /// </summary>
        /// <param name="sender">The sensor sends the event</param>
        /// <param name="x">X angle</param>
        /// <param name="y">Y angle</param>
        /// <param name="z">Z angle</param>
        public delegate Task GyroEventHandler(LPF2Device sender, double x, double y, double z);

        /// <summary>
        /// Gyro event handler
        /// </summary>
        public event GyroEventHandler? OnGyroEvent;

        internal override async Task ReceiveMessage(Message msg)
        {
            if (OnGyroEvent != null)
            {
                var x = Math.Round(7.0 * msg.Payload[1] / 100);
                var y = Math.Round(7.0 * msg.Payload[3] / 100);
                var z = Math.Round(7.0 * msg.Payload[5] / 100);
                await OnGyroEvent(this, x, y, z);
            }
        }
    }
}