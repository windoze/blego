using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Tacho Motor with PID control of speed/angle
    /// </summary>
    public class AbsoluteMotor : TachoMotor
    {
        private const byte MODE_ABSOLUTE = 0x03;

        internal AbsoluteMotor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
        }

        public delegate Task AbsoluteHandler(AbsoluteMotor sender, int angle);

        public event AbsoluteHandler? OnPositionChange;

        internal override async Task ReceiveMessage(Message msg)
        {
            await base.ReceiveMessage(msg);

            if (Mode == MODE_ABSOLUTE)
            {
                var angle = NormalizeAngle(BitConverter.ToInt16(msg.Payload.Slice(1)));

                if (OnPositionChange != null)
                {
                    await OnPositionChange(this, angle);
                }
            }
        }

        private static int NormalizeAngle(short angle)
        {
            if (angle >= 180)
            {
                return angle - (360 * ((angle + 180) / 360));
            }

            if (angle < -180)
            {
                return angle + (360 * ((180 - angle) / 360));
            }

            return angle;
        }

        public async Task RotateByDegrees(int degrees, int speed)
        {
            // TODO:
        }

        public async Task RotateByDegrees(int degrees, int speed1, int speed2)
        {
            if (!IsVirtualPort)
            {
                throw new LFP2DeviceError("Only virtual port supports multiple speeds.");
            }

            // TODO:
        }
    }
}