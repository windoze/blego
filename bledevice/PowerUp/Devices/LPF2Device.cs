using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;
using Serilog;

// ReSharper disable InconsistentNaming

namespace bledevice.PowerUp.Devices
{
    public class LFP2DeviceError : LPF2Error
    {
        public LFP2DeviceError(string message) : base(message)
        {
        }
    }

    public class LPF2Device
    {
        public LPF2DeviceType Type { get; private set; }
        public LPF2Hub Hub { get; private set; }
        public int Port { get; private set; }
        public string PortName { get; private set; }
        public byte Mode { get; set; }
        public bool IsVirtualPort { get; internal set; } = false;
        
        protected bool _connected = false;
        protected bool _buzy = false;

        internal LPF2Device(LPF2Hub hub, int port, LPF2DeviceType type)
        {
            Hub = hub;
            Port = port;
            Type = type;
            PortName = Hub.GetPortName(Port);
        }

        internal virtual async Task Initialize()
        {
        }

        internal virtual async Task ReceiveMessage(Message msg)
        {
        }

        internal virtual async Task LastActionFinished()
        {
        }

        public delegate Task DeviceEventHandler(LPF2Hub hub, LPF2Device device, int portId, byte mode, byte type);

        public event DeviceEventHandler? OnDeviceEvent = null;

        public async Task Subscribe(DeviceEventHandler handler)
        {
            OnDeviceEvent += handler;
        }

        public async Task Unsubscribe(DeviceEventHandler handler)
        {
            OnDeviceEvent -= handler;
        }

        public async Task EnableNotification(byte mode)
        {
            await Hub.EnablePortValueNotification(Port, mode);
        }

        public async Task DisableNotification(byte mode)
        {
            await Hub.DisablePortValueNotification(Port, mode);
        }

        // Factory
        internal static LPF2Device? CreateInstance(LPF2Hub hub, LPF2DeviceType type, int portId)
        {
            Log.Debug($"Creating LPF2Device(type={type}, port={portId})...");
            switch (type)
            {
                // case LPF2DeviceType.UNKNOWN:
                //     break;
                case LPF2DeviceType.SIMPLE_MEDIUM_LINEAR_MOTOR:
                    break;
                case LPF2DeviceType.TRAIN_MOTOR:
                    break;
                case LPF2DeviceType.LIGHT:
                    break;
                case LPF2DeviceType.VOLTAGE_SENSOR:
                    return new VoltageSensor(hub, portId);
                case LPF2DeviceType.CURRENT_SENSOR:
                    return new CurrentSensor(hub, portId);
                case LPF2DeviceType.PIEZO_BUZZER:
                    break;
                case LPF2DeviceType.HUB_LED:
                    return new HubLED(hub, portId);
                case LPF2DeviceType.TILT_SENSOR:
                    break;
                case LPF2DeviceType.MOTION_SENSOR:
                    break;
                case LPF2DeviceType.COLOR_DISTANCE_SENSOR:
                    return new ColorDistanceSensor(hub, portId);
                case LPF2DeviceType.MEDIUM_LINEAR_MOTOR:
                    return new MediumLinearMotor(hub, portId);
                case LPF2DeviceType.MOVE_HUB_MEDIUM_LINEAR_MOTOR:
                    return new MoveHubMediumLinearMotor(hub, portId);
                case LPF2DeviceType.MOVE_HUB_TILT_SENSOR:
                    return new MoveHubTiltSensor(hub, portId);
                case LPF2DeviceType.DUPLO_TRAIN_BASE_MOTOR:
                    break;
                case LPF2DeviceType.DUPLO_TRAIN_BASE_SPEAKER:
                    break;
                case LPF2DeviceType.DUPLO_TRAIN_BASE_COLOR_SENSOR:
                    break;
                case LPF2DeviceType.DUPLO_TRAIN_BASE_SPEEDOMETER:
                    break;
                case LPF2DeviceType.TECHNIC_LARGE_LINEAR_MOTOR:
                    break;
                case LPF2DeviceType.TECHNIC_XLARGE_LINEAR_MOTOR:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_ANGULAR_MOTOR:
                    break;
                case LPF2DeviceType.TECHNIC_LARGE_ANGULAR_MOTOR:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_HUB_GESTURE_SENSOR:
                    break;
                case LPF2DeviceType.REMOTE_CONTROL_BUTTON:
                    break;
                case LPF2DeviceType.REMOTE_CONTROL_RSSI:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_HUB_ACCELEROMETER:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_HUB_GYRO_SENSOR:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_HUB_TILT_SENSOR:
                    break;
                case LPF2DeviceType.TECHNIC_MEDIUM_HUB_TEMPERATURE_SENSOR:
                    break;
                case LPF2DeviceType.TECHNIC_COLOR_SENSOR:
                    break;
                case LPF2DeviceType.TECHNIC_DISTANCE_SENSOR:
                    break;
                case LPF2DeviceType.TECHNIC_FORCE_SENSOR:
                    break;
                default:
                    Log.Warning("Unknown device.");
                    break;
            }

            return null;
        }

        public override string ToString()
        {
            return $"LPF2Device(type={Type}, hub={Hub}, port={PortName}[{Port}])";
        }
    }
}