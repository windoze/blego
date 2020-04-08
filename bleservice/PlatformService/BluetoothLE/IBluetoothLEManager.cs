// Code taken from https://github.com/imurvai/brickcontroller2
// Copyrighted by István Murvai
// The license of the original code is unclear

// ReSharper disable InconsistentNaming

using System;
using System.Threading;
using System.Threading.Tasks;

namespace bleservice.PlatformService.BluetoothLE
{
    public interface IBluetoothLEManager
    {
        bool IsBluetoothLESupported { get; }
        bool IsBluetoothOn { get; }

        Task<bool> ScanDevicesAsync(Action<ScanResult> scanCallback, CancellationToken token);

        IBluetoothLEDevice GetKnownDevice(string address);
    }
}