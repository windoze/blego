using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Control+ Large Motor, Part ID 22169 
    /// </summary>
    public class TechnicLargeLinearMotor: AbsoluteMotor
    {
        internal TechnicLargeLinearMotor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.TECHNIC_LARGE_LINEAR_MOTOR)
        {
        }
    }
}