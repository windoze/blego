using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Powered Up LED Light, Part ID 88005
    /// </summary>
    public class Light : LPF2Device
    {
        internal Light(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.LIGHT)
        {
        }

        /// <summary>
        /// Set the brightness
        /// </summary>
        /// <param name="brightness">Brightness from 0 to 100</param>
        /// <returns></returns>
        public async Task SetBrightness(byte brightness)
        {
            await Hub.Send(Message.MessageType.PortOutputCommand, new byte[] {(byte) Port, 0x11, 0x51, 0x00, brightness});
        }
    }
}