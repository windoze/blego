using bledevice.PowerUp.Hubs;

namespace bledevice.PowerUp.Devices
{
    public class MediumLinearMotor : TachoMotor
    {
        public MediumLinearMotor(LPF2Hub hub, int port)
            : base(hub, port, LPF2DeviceType.MEDIUM_LINEAR_MOTOR)
        {
            Mode = MODE_ROTATION;
        }
    }
}