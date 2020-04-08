using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Control+ XLarge Motor, Part ID 22172 
    /// </summary>
    public class TechnicXLargeLinearMotor : AbsoluteMotor
    {
        internal TechnicXLargeLinearMotor(LPF2Hub hub, int port) : base(hub, port,
            LPF2DeviceType.TECHNIC_XLARGE_LINEAR_MOTOR)
        {
        }
    }
}