using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Boost MoveHub Internal Tilt Sensor 
    /// </summary>
    public class MoveHubTiltSensor : TiltSensor
    {
        internal MoveHubTiltSensor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.MOVE_HUB_TILT_SENSOR)
        {
        }
    }
}