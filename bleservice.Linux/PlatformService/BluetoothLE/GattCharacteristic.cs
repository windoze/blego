using System;
using bleservice.PlatformService.BluetoothLE;
using HashtagChris.DotNetBlueZ;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class GattCharacteristic : IGattCharacteristic
    {
        #region Overrides

        public Guid Uuid { get; }

        #endregion

        private IGattCharacteristic1 _characteristic;

        public GattCharacteristic(IGattCharacteristic1 characteristic)
        {
            _characteristic = characteristic;
            var t = _characteristic.GetUUIDAsync();
            t.Wait();
            Uuid = Guid.Parse(t.Result);
        }
    }
}