using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Devices;
using HashtagChris.DotNetBlueZ;
using Serilog;

namespace bledevice.PoweredUp.Hubs
{
    /// <summary>
    /// Boost MoveHub, Part ID 88006
    /// </summary>
#pragma warning disable 8603  
    public class MoveHub : LPF2Hub
    {
        #region Properties

        /// <summary>
        /// Internal Tilt Sensor
        /// </summary>
        public MoveHubTiltSensor TiltSensor => _devices[58] as MoveHubTiltSensor;

        /// <summary>
        /// Internal Tacho Motor on Port A
        /// </summary>
        public MoveHubMediumLinearMotor MotorA => _devices[0] as MoveHubMediumLinearMotor;


        /// <summary>
        /// Internal Tacho Motor on Port B
        /// </summary>
        public MoveHubMediumLinearMotor MotorB => _devices[1] as MoveHubMediumLinearMotor;

        /// <summary>
        /// Virtual Motor Group to control Motor A+B at same time
        /// </summary>
        public MoveHubMediumLinearMotor MotorAB => _devices[16] as MoveHubMediumLinearMotor;

        #endregion

        internal MoveHub(IDevice1 device) : base(device)
        {
            // This is a Boost MoveHub
            HubType = LPF2HubType.MOVE_HUB;

            PortIdMap[0] = "A";
            PortIdMap[1] = "B";
            PortIdMap[2] = "C";
            PortIdMap[3] = "D";
            PortIdMap[50] = "HUB_LED";
            PortIdMap[58] = "TILT_SENSOR";
            PortIdMap[59] = "CURRENT_SENSOR";
            PortIdMap[60] = "VOLTAGE_SENSOR";
        }

        #region Factory

        /// <summary>
        /// Scan and connect to a MoveHub
        /// </summary>
        /// <param name="timeout">The scan will stop after the period</param>
        /// <returns>MoveHub instance if the device is successfully found and connected</returns>
        public static async Task<MoveHub?> ScanAndConnect(TimeSpan? timeout = null)
        {
            var device = await ScanAndConnectInternal(new ScanFilter(name: "LEGO Move Hub"), timeout);
            if (device == null) return null;
            var ret = new MoveHub(device);
            await ret.Initialize();
            return ret;
        }

        /// <summary>
        /// Connect to a MoveHub at specified MAC address
        /// </summary>
        /// <param name="address">The MAC address</param>
        /// <param name="timeout">The scan will stop after the period</param>
        /// <returns>MoveHub instance if the device is successfully found and connected</returns>
        public new static async Task<MoveHub?> Connect(string address, TimeSpan? timeout = null)
        {
            var device = await ScanAndConnectInternal(new ScanFilter(address), timeout);
            if (device == null) return null;
            var ret = new MoveHub(device);
            await ret.Initialize();
            return ret;
        }

        #endregion
    }
}