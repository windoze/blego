// Code taken from https://github.com/imurvai/brickcontroller2
// Copyrighted by István Murvai
// The license of the original code is unclear

using System;

namespace bleservice.PlatformService.BluetoothLE
{
    public interface IGattCharacteristic
    {
        Guid Uuid { get; }
    }
}