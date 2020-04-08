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
    public class MoveHub : LPF2Hub
    {
        #region Properties

        /// <summary>
        /// Built-in Current Sensor
        /// </summary>
        public CurrentSensor CurrentSensor { get; private set; }

        /// <summary>
        /// Built-in Voltage Sensor
        /// </summary>
        public VoltageSensor VoltageSensor { get; private set; }

        /// <summary>
        /// Integrated RGD LED Light
        /// </summary>
        public HubLED LED { get; private set; }

        /// <summary>
        /// Internal Tilt Sensor
        /// </summary>
        public MoveHubTiltSensor TiltSensor { get; private set; }

        /// <summary>
        /// Internal Tacho Motor on Port A
        /// </summary>
        public MoveHubMediumLinearMotor MotorA { get; private set; }


        /// <summary>
        /// Internal Tacho Motor on Port B
        /// </summary>
        public MoveHubMediumLinearMotor MotorB { get; private set; }

        /// <summary>
        /// Virtual Motor Group to control Motor A+B at same time
        /// </summary>
        public MoveHubMediumLinearMotor MotorAB { get; private set; }

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

        protected override async Task SetupHub()
        {
            Log.Information("Move Hub connected");
            LED = _devices[50] as HubLED;
            TiltSensor = _devices[58] as MoveHubTiltSensor;
            MotorA = _devices[0] as MoveHubMediumLinearMotor;
            MotorB = _devices[1] as MoveHubMediumLinearMotor;
            MotorAB = _devices[16] as MoveHubMediumLinearMotor;
            CurrentSensor = _devices[59] as CurrentSensor;
            VoltageSensor = _devices[60] as VoltageSensor;
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