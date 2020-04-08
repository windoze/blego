using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bleservice.PlatformService.BluetoothLE;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class BluetoothLEDevice: IBluetoothLEDevice
    {
        public string Address { get; }
        public BluetoothLEDeviceState State { get; }

        public Task<IEnumerable<IGattService>> ConnectAndDiscoverServicesAsync(bool autoConnect, Action<Guid, byte[]> onCharacteristicChanged, Action<IBluetoothLEDevice> onDeviceDisconnected,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnableNotificationAsync(IGattCharacteristic characteristic, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsync(IGattCharacteristic characteristic, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteAsync(IGattCharacteristic characteristic, byte[] data, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteNoResponseAsync(IGattCharacteristic characteristic, byte[] data, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}