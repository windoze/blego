using System;
using System.Threading.Tasks;
using HashtagChris.DotNetBlueZ;

namespace bledevice.GeneralServices
{
    public class DeviceInformationService : GattService
    {
        private static readonly Guid SYSTEM_ID_UUID = Guid.Parse("00002A23-0000-1000-8000-00805F9B34FB");
        private static readonly Guid MODEL_NUMBER_UUID = Guid.Parse("00002A24-0000-1000-8000-00805F9B34FB");
        private static readonly Guid SERIAL_NUMBER_UUID = Guid.Parse("00002A25-0000-1000-8000-00805F9B34FB");
        private static readonly Guid FIRMWARE_REVISION_UUID = Guid.Parse("00002A26-0000-1000-8000-00805F9B34FB");
        private static readonly Guid HARDWARE_REVISION_UUID = Guid.Parse("00002A27-0000-1000-8000-00805F9B34FB");
        private static readonly Guid SOFTWARE_REVISION_UUID = Guid.Parse("00002A28-0000-1000-8000-00805F9B34FB");
        private static readonly Guid MANUFACTURER_NAME_UUID = Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB");

        public DeviceInformationService(IGattService1 service) : base(service)
        {
        }

        public async Task<string> GetSystemId()
        {
            return await ReadString(SYSTEM_ID_UUID);
        }

        public async Task<string> GetModelNumber()
        {
            return await ReadString(MODEL_NUMBER_UUID);
        }

        public async Task<string> GetSerialNumber()
        {
            return await ReadString(SERIAL_NUMBER_UUID);
        }

        public async Task<string> GetFirmwareRevision()
        {
            return await ReadString(FIRMWARE_REVISION_UUID);
        }

        public async Task<string> GetHardwareRevision()
        {
            return await ReadString(HARDWARE_REVISION_UUID);
        }

        public async Task<string> GetSoftwareRevision()
        {
            return await ReadString(SOFTWARE_REVISION_UUID);
        }

        public async Task<string> GetManufacturerName()
        {
            return await ReadString(MANUFACTURER_NAME_UUID);
        }
    }
}