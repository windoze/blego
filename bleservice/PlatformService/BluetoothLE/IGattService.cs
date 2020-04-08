// Code taken from https://github.com/imurvai/brickcontroller2
// Copyrighted by István Murvai
// The license of the original code is unclear

using System;
using System.Collections.Generic;

namespace bleservice.PlatformService.BluetoothLE
{
    public interface IGattService
    {
        Guid Uuid { get; }
        IEnumerable<IGattCharacteristic> Characteristics { get; }
    }
}