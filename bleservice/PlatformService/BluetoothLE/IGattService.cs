using System;
using System.Collections.Generic;

namespace bledevice.PlatformService.BluetoothLE
{
    public interface IGattService
    {
        Guid Uuid { get; }
        IEnumerable<IGattCharacteristic> Characteristics { get; }
    }
}