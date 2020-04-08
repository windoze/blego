using System;
using System.Collections.Generic;
using bleservice.PlatformService.BluetoothLE;
using HashtagChris.DotNetBlueZ;

namespace bleservice.Linux.PlatformService.BluetoothLE
{
    public class GattService : IGattService
    {
        #region Overrides

        public Guid Uuid { get; }
        public IEnumerable<IGattCharacteristic> Characteristics { get; }

        #endregion

        private IGattService1 _service;

        public GattService(IGattService1 service)
        {
            _service = service;
            var t = _service.GetUUIDAsync();
            t.Wait();
            Uuid = Guid.Parse(t.Result);
            var t1 = _service.GetAllAsync();
            t.Wait();
        }
    }
}