using System.Threading.Tasks;

namespace bledevice.PoweredUp.Devices
{
    public interface ITiltSensor : ISensor
    {
        /// <summary>
        /// The tilt event handler
        /// </summary>
        /// <param name="sender">The tilt sensor sends the event</param>
        /// <param name="x">Tilt degrees of X axis</param>
        /// <param name="y">Tilt degrees of Y axis</param>
        public delegate Task TiltHandler(ITiltSensor sender, int x, int y);

        /// <summary>
        /// The tilt event handler
        /// </summary>
        public event TiltHandler? OnTilt;
    }
}