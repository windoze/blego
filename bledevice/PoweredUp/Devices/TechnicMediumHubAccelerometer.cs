using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Control+ Hub Internal Accelerometer  
    /// </summary>
    public class TechnicMediumHubAccelerometer : Accelerometer
    {
        internal TechnicMediumHubAccelerometer(LPF2Hub hub, int port) : base(hub, port,
            LPF2DeviceType.TECHNIC_MEDIUM_HUB_ACCELEROMETER)
        {
        }
    }
}