using System.Threading.Tasks;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;

// ReSharper disable InconsistentNaming
namespace bledevice.PowerUp.Devices
{
    public class HubLED : LPF2Device
    {
        private const byte MODE_COLOR = 0x00;
        private const byte MODE_RGB = 0x01;

        internal HubLED(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.HUB_LED)
        {
            Mode = MODE_COLOR;
        }

        public async Task SetColor(Color color)
        {
            await Hub.EnablePortValueNotification(Port, MODE_COLOR);
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, MODE_COLOR, (byte) color});
        }

        public async Task SetRGB(byte red, byte green, byte blue)
        {
            await Hub.EnablePortValueNotification(Port, MODE_RGB);
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, MODE_RGB, red, green, blue});
        }
    }
}