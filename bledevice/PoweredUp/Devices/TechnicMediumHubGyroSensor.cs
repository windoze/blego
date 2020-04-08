using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Control+ Hub Internal Gyro Sensor  
    /// </summary>
    public class TechnicMediumHubGyroSensor : GyroSensor
    {
        internal TechnicMediumHubGyroSensor(LPF2Hub hub, int port) : base(hub, port,
            LPF2DeviceType.TECHNIC_MEDIUM_HUB_GYRO_SENSOR)
        {
        }
    }
}