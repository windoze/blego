using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Simple Motor with PWM power control
    /// </summary>
    public abstract class BasicMotor : LPF2Device, IMotor
    {
        internal BasicMotor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
        }

        /// <summary>
        /// Set PWM duty cycle
        /// </summary>
        /// <param name="power">power range is [-100, 100], negative value means the motor will rotate in the opposite direction.</param>
        /// <returns></returns>
        public async Task SetPower(int power)
        {
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, 0x00, MapSpeed(power)});
        }

        /// <summary>
        /// Stop the motor by stopping the power supply
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            await SetPower(0);
        }

        /// <summary>
        /// Brake the motor at the current position
        /// </summary>
        /// <returns></returns>
        public async Task Brake()
        {
            // Brake
            await SetPower(127);
        }

        protected static byte MapSpeed(int value)
        {
            if (value == 127)
            {
                return 127;
            }

            if (value > 100)
            {
                value = 100;
            }
            else if (value < -100)
            {
                value = -100;
            }

            return (byte) ((sbyte) value);
        }
    }
}