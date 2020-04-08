// ReSharper disable InconsistentNaming

using System;
using System.Threading;
using System.Threading.Tasks;

namespace bledevice.PlatformService.BluetoothLE
{
    public interface IBluetoothLEManager
    {
        bool IsBluetoothLESupported { get; }
        bool IsBluetoothOn { get; }

        Task<bool> ScanDevicesAsync(Action<ScanResult> scanCallback, CancellationToken token);

        IBluetoothLEDevice GetKnownDevice(string address);
    }
}