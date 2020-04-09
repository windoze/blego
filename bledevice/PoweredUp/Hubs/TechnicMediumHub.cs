using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Devices;
using HashtagChris.DotNetBlueZ;
using Serilog;

// ReSharper disable InconsistentNaming

namespace bledevice.PoweredUp.Hubs
{
    /// <summary>
    /// Technic Medium Hub (Control+ Hub), Part ID 22127
    /// </summary>
#pragma warning disable 8603  
    public class TechnicMediumHub : LPF2Hub
    {
        #region Properties

        /// <summary>
        /// Built-in Gyro Sensor
        /// </summary>
        public GyroSensor GyroSensor => _devices[98] as GyroSensor;

        /// <summary>
        /// Built-in Accelerometer
        /// </summary>
        public Accelerometer Accelerometer => _devices[97] as Accelerometer;

        /// <summary>
        /// Built-in TiltSensor
        /// </summary>
        public TechnicMediumHubTiltSensor TiltSensor => _devices[99] as TechnicMediumHubTiltSensor;

        #endregion

        internal TechnicMediumHub(IDevice1 device) : base(device)
        {
            // This is a Technic Medium Hub
            HubType = LPF2HubType.TECHNIC_MEDIUM_HUB;

            PortIdMap[0] = "A";
            PortIdMap[1] = "B";
            PortIdMap[2] = "C";
            PortIdMap[3] = "D";
            PortIdMap[50] = "HUB_LED";
            PortIdMap[59] = "CURRENT_SENSOR";
            PortIdMap[60] = "VOLTAGE_SENSOR";
            // What are these?
            // PortIdMap[61] = "TEMPERATURE_SENSOR";
            // PortIdMap[96] = "TEMPERATURE_SENSOR";
            PortIdMap[97] = "ACCELEROMETER";
            PortIdMap[98] = "GYRO_SENSOR";
            PortIdMap[99] = "TILT_SENSOR";
        }

        #region Factory

        /// <summary>
        /// Scan and connect to a TechnicMediumHub
        /// </summary>
        /// <param name="timeout">The scan will stop after the period</param>
        /// <returns>TechnicMediumHub instance if the device is successfully found and connected</returns>
        public static async Task<TechnicMediumHub?> ScanAndConnect(TimeSpan? timeout = null)
        {
            var device = await ScanAndConnectInternal(new ScanFilter(name: "Technic Hub"), timeout);
            if (device == null) return null;
            var ret = new TechnicMediumHub(device);
            await ret.Initialize();
            return ret;
        }

        /// <summary>
        /// Connect to a TechnicMediumHub at specified MAC address
        /// </summary>
        /// <param name="address">The MAC address</param>
        /// <param name="timeout">The scan will stop after the period</param>
        /// <returns>TechnicMediumHub instance if the device is successfully found and connected</returns>
        public new static async Task<TechnicMediumHub?> Connect(string address, TimeSpan? timeout = null)
        {
            var device = await ScanAndConnectInternal(new ScanFilter(address), timeout);
            if (device == null) return null;
            var ret = new TechnicMediumHub(device);
            await ret.Initialize();
            return ret;
        }

        #endregion
    }
}