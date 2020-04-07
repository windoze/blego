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

        public BrakingStyles BrakingStyle { get; set; }

        public delegate Task RotateHandler(TachoMotor sender, int degrees);

        public event RotateHandler? OnRotate;

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
            // TODO:
        }

        // For Virtual Port
        public async Task SetSpeed(int speed1, int speed2, TimeSpan? duration = null)
        {
            if (!IsVirtualPort)
            {
                throw new LFP2DeviceError("Only virtual port supports multiple speeds.");
            }

            // TODO:
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