using System;

namespace bledevice.PlatformService.BluetoothLE
{
    public interface IGattCharacteristic
    {
        Guid Uuid { get; }
    }
}