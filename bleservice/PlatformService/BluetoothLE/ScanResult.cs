// Code taken from https://github.com/imurvai/brickcontroller2
// Copyrighted by István Murvai
// The license of the original code is unclear

using System.Collections.Generic;

namespace bleservice.PlatformService.BluetoothLE
{
    public class ScanResult
    {
        public ScanResult(string deviceName, string deviceAddress, IDictionary<byte, byte[]> advertisementData)
        {
            DeviceName = deviceName;
            DeviceAddress = deviceAddress;
            AdvertisementData = advertisementData;
        }

        public string DeviceName { get; }
        public string DeviceAddress { get; }
        public IDictionary<byte, byte[]> AdvertisementData { get; }
    }
}