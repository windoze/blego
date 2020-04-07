using System.Threading.Tasks;

namespace bledevice.PowerUp.Devices
{
    public interface ITiltSensor : ISensor
    {
        public delegate Task TiltHandler(ITiltSensor sender, int x, int y);

        public event TiltHandler? OnTilt;
    }
}