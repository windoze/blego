using bledevice.PoweredUp.Hubs;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Boost MoveHub internal tacho motors
    /// </summary>
    public class MoveHubMediumLinearMotor : TachoMotor
    {
        public MoveHubMediumLinearMotor(LPF2Hub hub, int port)
            : base(hub, port, LPF2DeviceType.MOVE_HUB_MEDIUM_LINEAR_MOTOR)
        {
        }
    }
}