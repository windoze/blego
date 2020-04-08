using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Control+ Hub Internal Tilt Sensor  
    /// </summary>
    public class TechnicMediumHubTiltSensor : LPF2Device, ISensor
    {
        private const byte MODE_TILT = 0x00;

        public byte DefaultMode => MODE_TILT;
        public bool AutoSubscribe => true;

        internal TechnicMediumHubTiltSensor(LPF2Hub hub, int port) : base(hub, port,
            LPF2DeviceType.TECHNIC_MEDIUM_HUB_TILT_SENSOR)
        {
            Mode = MODE_TILT;
        }

        /// <summary>
        /// The tilt event handler
        /// </summary>
        /// <param name="sender">The tilt sensor sends the event</param>
        /// <param name="x">Tilt degrees of X axis</param>
        /// <param name="y">Tilt degrees of Y axis</param>
        /// <param name="z">Tilt degrees of Z axis</param>
        public delegate Task TiltHandler(TechnicMediumHubTiltSensor sender, int x, int y, int z);

        /// <summary>
        /// The tilt event handler
        /// </summary>
        public event TiltHandler? OnTilt;

        internal override async Task ReceiveMessage(Message msg)
        {
            if (OnTilt != null)
            {
                var z = -BitConverter.ToInt16(msg.Payload.Slice(1));
                var x = BitConverter.ToInt16(msg.Payload.Slice(3));
                var y = BitConverter.ToInt16(msg.Payload.Slice(5));
                if (OnTilt != null)
                {
                    await OnTilt(this, x, y, z);
                }
            }
        }
    }
}