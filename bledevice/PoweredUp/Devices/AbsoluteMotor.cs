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
            Mode = MODE_ABSOLUTE;
        }

        public byte DefaultMode => MODE_ABSOLUTE; // Absolute
        public bool AutoSubscribe => false;

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
            else
            {
                await base.ReceiveMessage(msg);
            }
        }

        private static int NormalizeAngle(int angle)
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

        public async Task GoToAngle(int angle, int speed)
        {
            var angleBytes = BitConverter.GetBytes(NormalizeAngle(angle));
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[]
                {
                    (byte) Port, 0x11, 0x0d, angleBytes[0], angleBytes[1], angleBytes[2], angleBytes[3],
                    MapSpeed(speed), 0x64, (byte) BrakingStyle, 0x00
                });
        }

        public async Task GoToAngle(int angle1, int angle2, int speed)
        {
            if (!IsVirtualPort)
            {
                throw new LFP2DeviceError("Only virtual port supports multiple angles.");
            }

            var angle1Bytes = BitConverter.GetBytes(NormalizeAngle(angle1));
            var angle2Bytes = BitConverter.GetBytes(NormalizeAngle(angle2));
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[]
                {
                    (byte) Port, 0x11, 0x0e,
                    angle1Bytes[0], angle1Bytes[1], angle1Bytes[2], angle1Bytes[3],
                    angle2Bytes[0], angle2Bytes[1], angle2Bytes[2], angle2Bytes[3],
                    MapSpeed(speed), 0x64, (byte) BrakingStyle, 0x00
                });
        }
    }
}