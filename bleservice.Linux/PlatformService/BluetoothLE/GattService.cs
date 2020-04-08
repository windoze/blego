using System;
using bleservice.PlatformService.BluetoothLE;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class GattService: IGattCharacteristic
    {
        public Guid Uuid { get; }
    }
}