using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;

namespace bledevice.PowerUp.Devices
{
    public abstract class TachoMotor : BasicMotor, ISensor // Tacho Motors are sensors
    {
        public enum BrakingStyles : byte
        {
            FLOAT = 0,
            BRAKE = 126,
            HOLD = 127,
        }

        internal const byte MODE_ROTATION = 0x02;

        internal TachoMotor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
            Mode = MODE_ROTATION;
        }

        public byte DefaultMode => MODE_ROTATION; // Rotation
        public bool AutoSubscribe => false;

        public BrakingStyles BrakingStyle { get; set; } = BrakingStyles.HOLD;

        public delegate Task RotateHandler(TachoMotor sender, int degrees);

        public event RotateHandler? OnRotate;

        internal override async Task Initialize()
        {
        }

        internal override async Task ReceiveMessage(Message msg)
        {
            if (Mode == MODE_ROTATION)
            {
                var degrees = BitConverter.ToInt32(msg.Payload.Slice(1));
                if (OnRotate != null)
                {
                    await OnRotate(this, degrees);
                }
            }
        }

        // For Single Port
        public async Task SetSpeed(int speed, TimeSpan? duration = null)
        {
            if (duration == null)
            {
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[] {(byte) Port, 0x11, 0x07, MapSpeed(speed), 0x64, 0x03, 0x64, (byte) BrakingStyle, 0x00});
            }
            else
            {
                var timeBytes = BitConverter.GetBytes((ushort) duration.Value.TotalMilliseconds);
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[] {(byte) Port, 0x11, 0x09, timeBytes[0], timeBytes[1], MapSpeed(speed), 0x64, (byte) BrakingStyle, 0x00});
            }
        }

        // For Virtual Port
        public async Task SetSpeed(int speed1, int speed2, TimeSpan? duration = null)
        {
            if (!IsVirtualPort)
            {
                throw new LFP2DeviceError("Only virtual port supports multiple speeds.");
            }

            if (duration == null)
            {
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[] {(byte) Port, 0x11, 0x08, MapSpeed(speed1), MapSpeed(speed2), 0x64, (byte) BrakingStyle, 0x00});
            }
            else
            {
                var timeBytes = BitConverter.GetBytes((ushort) duration.Value.TotalMilliseconds);
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[] {(byte) Port, 0x11, 0x0A, timeBytes[0], timeBytes[1], MapSpeed(speed1), MapSpeed(speed2), 0x64, (byte) BrakingStyle, 0x00});
            }
        }
    }
}