using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Accelerometer
    /// </summary>
    public class Accelerometer : LPF2Device, ISensor
    {
        private const byte MODE_ACCEL = 0x00;

        internal Accelerometer(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
            Mode = MODE_ACCEL;
        }

        public byte DefaultMode => MODE_ACCEL;
        public bool AutoSubscribe => true;

        /// <summary>
        /// Event handler for accelerations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="x">Acceleration on X axis</param>
        /// <param name="y">Acceleration on Y axis</param>
        /// <param name="z">Acceleration on Z axis</param>
        public delegate Task AccelEventHandler(LPF2Device sender, double x, double y, double z);

        public event AccelEventHandler? OnAccelEvent;

        internal override async Task ReceiveMessage(Message msg)
        {
            if (OnAccelEvent != null)
            {
                var x = Math.Round(BitConverter.ToInt16(msg.Payload.Slice(1)) / 4.096);
                var y = Math.Round(BitConverter.ToInt16(msg.Payload.Slice(3)) / 4.096);
                var z = Math.Round(BitConverter.ToInt16(msg.Payload.Slice(5)) / 4.096);
                await OnAccelEvent(this, x, y, z);
            }
        }
    }
}