using System.Threading.Tasks;

namespace bledevice.PowerUp.Devices
{
    public interface ISensor
    {
        byte DefaultMode { get; }
        bool AutoSubscribe { get; }
    }
}