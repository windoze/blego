using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    public class TiltSensor : LPF2Device, ISensor
    {
        private const byte MODE_TILT = 0x00;

        internal TiltSensor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
            Mode = MODE_TILT;
        }

        public byte DefaultMode => MODE_TILT;
        public bool AutoSubscribe => true;

        /// <summary>
        /// The tilt event handler
        /// </summary>
        /// <param name="sender">The tilt sensor sends the event</param>
        /// <param name="x">Tilt degrees of X axis</param>
        /// <param name="y">Tilt degrees of Y axis</param>
        public delegate Task TiltHandler(TiltSensor sender, int x, int y);

        /// <summary>
        /// The tilt event handler
        /// </summary>
        public event TiltHandler? OnTilt;

        internal override async Task ReceiveMessage(Message msg)
        {
            var x = -((sbyte) msg.Payload[1]);
            var y = (sbyte) msg.Payload[2];
            if (OnTilt != null)
            {
                await OnTilt(this, x, y);
            }
        }
    }
}