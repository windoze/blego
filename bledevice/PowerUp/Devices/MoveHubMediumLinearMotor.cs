using bledevice.PowerUp.Hubs;

namespace bledevice.PowerUp.Devices
{
    public class MoveHubMediumLinearMotor : TachoMotor
    {
        public MoveHubMediumLinearMotor(LPF2Hub hub, int port)
            : base(hub, port, LPF2DeviceType.MOVE_HUB_MEDIUM_LINEAR_MOTOR)
        {
            Mode = MODE_ROTATION;
        }
    }
}