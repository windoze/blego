using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Boost Medium Tacho Motor, Part ID 88008
    /// </summary>
    public class MediumLinearMotor : TachoMotor
    {
        public MediumLinearMotor(LPF2Hub hub, int port)
            : base(hub, port, LPF2DeviceType.MEDIUM_LINEAR_MOTOR)
        {
        }
    }
}