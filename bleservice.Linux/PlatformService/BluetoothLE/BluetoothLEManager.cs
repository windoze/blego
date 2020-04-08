using System;
using System.Threading;
using System.Threading.Tasks;
using bleservice.PlatformService.BluetoothLE;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class BluetoothLEManager: IBluetoothLEManager
    {
        public bool IsBluetoothLESupported { get; }
        public bool IsBluetoothOn { get; }
        public Task<bool> ScanDevicesAsync(Action<ScanResult> scanCallback, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public IBluetoothLEDevice GetKnownDevice(string address)
        {
            throw new NotImplementedException();
        }
    }
}