namespace bledevice.PoweredUp.Devices
{
    public interface ISensor
    {
        byte DefaultMode { get; }
        bool AutoSubscribe { get; }
    }
}