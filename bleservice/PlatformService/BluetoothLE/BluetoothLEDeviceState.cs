// Code taken from https://github.com/imurvai/brickcontroller2
// Copyrighted by István Murvai
// The license of the original code is unclear

// ReSharper disable InconsistentNaming
namespace bleservice.PlatformService.BluetoothLE
{
    public enum BluetoothLEDeviceState
    {
        Disconnected,
        Connecting,
        Discovering,
        Connected,
        Disconnecting
    }
}