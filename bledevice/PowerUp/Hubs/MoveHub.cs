using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Devices;
using HashtagChris.DotNetBlueZ;
using Serilog;

namespace bledevice.PowerUp.Hubs
{
    public class MoveHub : LPF2Hub
    {
        #region Properties

        public HubLED LED { get; private set; }
        public MoveHubTiltSensor TiltSensor { get; private set; }
        public MoveHubMediumLinearMotor MotorA { get; private set; }
        public MoveHubMediumLinearMotor MotorB { get; private set; }
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

        protected override async Task BleDeviceConnected()
        {
            await base.BleDeviceConnected();
            Log.Information("Move Hub connected");
            LED = _devices[50] as HubLED;
            TiltSensor = _devices[58] as MoveHubTiltSensor;
            MotorA = _devices[0] as MoveHubMediumLinearMotor;
            MotorB = _devices[1] as MoveHubMediumLinearMotor;
            MotorAB = _devices[16] as MoveHubMediumLinearMotor;
        }

        #region Factory

        public static async Task<MoveHub?> ScanAndConnect(TimeSpan? timeout = null)
        {
            var device = await ScanAndConnectInternal(new ScanFilter(name: "LEGO Move Hub"), timeout);
            if (device == null) return null;
            var ret = new MoveHub(device);
            await ret.Initialize();
            return ret;
        }

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