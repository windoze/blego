using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Tacho Motor with PWM power control and tacho
    /// </summary>
    public abstract class TachoMotor : BasicMotor, ISensor // Tacho Motors are sensors
    {
        public enum BrakingStyles : byte
        {
            /// <summary>
            /// Just stop power supply
            /// </summary>
            FLOAT = 0,

            /// <summary>
            /// The motor is shorted
            /// </summary>
            BRAKE = 126,

            /// <summary>
            /// The motor actively keeps the current position
            /// </summary>
            HOLD = 127,
        }

        internal const byte MODE_ROTATION = 0x02;

        internal TachoMotor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
            Mode = MODE_ROTATION;
        }

        public byte DefaultMode => MODE_ROTATION; // Rotation
        public bool AutoSubscribe => false;

        /// <summary>
        /// How the motor should brake when it finishes the rotation.
        /// </summary>
        public BrakingStyles BrakingStyle { get; set; } = BrakingStyles.HOLD;

        /// <summary>
        /// Rotate event handler
        /// </summary>
        /// <param name="sender">The motor sends the event</param>
        /// <param name="degrees">how many degrees the motor rotated.</param>
        public delegate Task RotateHandler(TachoMotor sender, int degrees);

        /// <summary>
        /// The event handler for motor rotation
        /// </summary>
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

        /// <summary>
        /// Set the speed of the motor, if the device is a virtual port, set speed for both motor at the same time.
        /// The <see cref="BrakingStyle"/> property controls the behavior when motor finishes the rotating.
        /// </summary>
        /// <param name="speed">The speed range is [-100, 100], negative value means the motor will rotate on the opposite direction.</param>
        /// <param name="duration">Time that the motor should keep rotating</param>
        /// <returns></returns>
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
                    new byte[]
                    {
                        (byte) Port, 0x11, 0x09, timeBytes[0], timeBytes[1], MapSpeed(speed), 0x64, (byte) BrakingStyle,
                        0x00
                    });
            }
        }

        /// <summary>
        /// Set the speed of the virtual motor group.
        /// </summary>
        /// <param name="speed1">Speed for the first motor in the group</param>
        /// <param name="speed2">Speed for the second motor in the group</param>
        /// <param name="duration">Time that the motors should keep rotating</param>
        /// <returns></returns>
        /// <exception cref="LFP2DeviceError">Exception thrown if the device is not a virtual port.</exception>
        public async Task SetSpeed(int speed1, int speed2, TimeSpan? duration = null)
        {
            if (!IsVirtualPort)
            {
                throw new LFP2DeviceError("Only virtual port supports multiple speeds.");
            }

            if (duration == null)
            {
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[]
                    {
                        (byte) Port, 0x11, 0x08, MapSpeed(speed1), MapSpeed(speed2), 0x64, (byte) BrakingStyle, 0x00
                    });
            }
            else
            {
                var timeBytes = BitConverter.GetBytes((ushort) duration.Value.TotalMilliseconds);
                await Hub.Send(Message.MessageType.PortOutputCommand,
                    new byte[]
                    {
                        (byte) Port, 0x11, 0x0A, timeBytes[0], timeBytes[1], MapSpeed(speed1), MapSpeed(speed2), 0x64,
                        (byte) BrakingStyle, 0x00
                    });
            }
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