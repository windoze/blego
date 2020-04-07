using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

// ReSharper disable InconsistentNaming
namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Color LED On all Powered Up Hubs
    /// </summary>
    public class HubLED : LPF2Device
    {
        private const byte MODE_COLOR = 0x00;
        private const byte MODE_RGB = 0x01;

        internal HubLED(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.HUB_LED)
        {
            Mode = MODE_COLOR;
        }

        /// <summary>
        /// Set LED Color, all colors can be found at <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns></returns>
        public async Task SetColor(Color color)
        {
            await Hub.EnablePortValueNotification(Port, MODE_COLOR);
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, MODE_COLOR, (byte) color});
        }

        /// <summary>
        /// Set LED Color by RGB
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <returns></returns>
        public async Task SetRGB(byte red, byte green, byte blue)
        {
            await Hub.EnablePortValueNotification(Port, MODE_RGB);
            await Hub.Send(Message.MessageType.PortOutputCommand,
                new byte[] {(byte) Port, 0x11, 0x51, MODE_RGB, red, green, blue});
        }
    }
}