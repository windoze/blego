using System;
using bleservice.PlatformService.BluetoothLE;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class GattCharacteristic: IGattCharacteristic
    {
        public Guid Uuid { get; }
    }
}