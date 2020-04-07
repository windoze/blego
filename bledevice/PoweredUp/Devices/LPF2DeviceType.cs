namespace bledevice.PoweredUp.Devices
{
    // ReSharper disable InconsistentNaming
    public enum LPF2DeviceType
    {
        UNKNOWN = 0,
        SIMPLE_MEDIUM_LINEAR_MOTOR = 1,
        TRAIN_MOTOR = 2,
        LIGHT = 8,
        VOLTAGE_SENSOR = 20,
        CURRENT_SENSOR = 21,
        PIEZO_BUZZER = 22,
        HUB_LED = 23,
        TILT_SENSOR = 34,
        MOTION_SENSOR = 35,
        COLOR_DISTANCE_SENSOR = 37,
        MEDIUM_LINEAR_MOTOR = 38,
        MOVE_HUB_MEDIUM_LINEAR_MOTOR = 39,
        MOVE_HUB_TILT_SENSOR = 40,
        DUPLO_TRAIN_BASE_MOTOR = 41,
        DUPLO_TRAIN_BASE_SPEAKER = 42,
        DUPLO_TRAIN_BASE_COLOR_SENSOR = 43,
        DUPLO_TRAIN_BASE_SPEEDOMETER = 44,
        TECHNIC_LARGE_LINEAR_MOTOR = 46, // Technic Control+
        TECHNIC_XLARGE_LINEAR_MOTOR = 47, // Technic Control+
        TECHNIC_MEDIUM_ANGULAR_MOTOR = 48, // Spike Prime
        TECHNIC_LARGE_ANGULAR_MOTOR = 49, // Spike Prime
        TECHNIC_MEDIUM_HUB_GESTURE_SENSOR = 54,
        REMOTE_CONTROL_BUTTON = 55,
        REMOTE_CONTROL_RSSI = 56,
        TECHNIC_MEDIUM_HUB_ACCELEROMETER = 57,
        TECHNIC_MEDIUM_HUB_GYRO_SENSOR = 58,
        TECHNIC_MEDIUM_HUB_TILT_SENSOR = 59,
        TECHNIC_MEDIUM_HUB_TEMPERATURE_SENSOR = 60,
        TECHNIC_COLOR_SENSOR = 61, // Spike Prime
        TECHNIC_DISTANCE_SENSOR = 62, // Spike Prime
        TECHNIC_FORCE_SENSOR = 63 // Spike Prime
    }
}