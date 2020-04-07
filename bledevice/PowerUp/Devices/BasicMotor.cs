using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;

namespace bledevice.PowerUp.Devices
{
    public abstract class BasicMotor : LPF2Device, IMotor
    {
        internal BasicMotor(LPF2Hub hub, int port, LPF2DeviceType type) : base(hub, port, type)
        {
        }

        public async Task SetPower(int power)
        {
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, 0x00, (byte) MapSpeed(power)});
        }

        public async Task Stop()
        {
            await SetPower(0);
        }

        public async Task Brake()
        {
            // Brake
            await SetPower(127);
        }

        private static sbyte MapSpeed(int value)
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

            return (sbyte) value;
        }
    }
}