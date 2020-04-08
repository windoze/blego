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
    public class TechnicMediumHub : LPF2Hub
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

        protected override async Task SetupHub()
        {
            Log.Information("Technic Medium Hub connected");
            LED = _devices[50] as HubLED;
            CurrentSensor = _devices[59] as CurrentSensor;
            VoltageSensor = _devices[60] as VoltageSensor;
        }
    }
}