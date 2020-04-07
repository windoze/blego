using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;
using Serilog;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Tilt Sensor inside of Boost MoveHub
    /// </summary>
    public class MoveHubTiltSensor : LPF2Device, ITiltSensor
    {
        private const byte MODE_TILT = 0x00;

        internal MoveHubTiltSensor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.MOVE_HUB_TILT_SENSOR)
        {
            Mode = MODE_TILT;
        }

        internal override async Task Initialize()
        {
            Log.Debug($"Tilt Sensor connected to port {Port} on MoveHub({Hub.PrimaryMACAddress}).");
        }

        internal override async Task ReceiveMessage(Message msg)
        {
            var x = -((sbyte) msg.Payload[1]);
            var y = (sbyte) msg.Payload[2];
            if (OnTilt != null)
            {
                await OnTilt(this, x, y);
            }
        }

        public byte DefaultMode => MODE_TILT;

        public bool AutoSubscribe => true;
        public event ITiltSensor.TiltHandler? OnTilt;
    }
}