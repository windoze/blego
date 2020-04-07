// ReSharper disable InconsistentNaming

namespace bledevice.PoweredUp.Hubs
{
    public enum Color : byte
    {
        BLACK = 0,
        PINK = 1,
        PURPLE = 2,
        BLUE = 3,
        LIGHT_BLUE = 4,
        CYAN = 5,
        GREEN = 6,
        YELLOW = 7,
        ORANGE = 8,
        RED = 9,
        WHITE = 10,
        NONE = 255
    }

    public enum ButtonState : byte
    {
        RELEASED = 0,
        UP = 1,
        PRESSED = 2,
        STOP = 127,
        DOWN = 255,
    }

    public enum BrakingStyle : byte
    {
        FLOAT = 0,
        HOLD = 126,
        BRAKE = 127
    }

    public enum DuploTrainBaseSound : byte
    {
        BRAKE = 3,
        STATION_DEPARTURE = 5,
        WATER_REFILL = 7,
        HORN = 9,
        STEAM = 10
    }

    public enum BLEManufacturerData : byte
    {
        DUPLO_TRAIN_BASE_ID = 32,
        MOVE_HUB_ID = 64,
        HUB_ID = 65,
        REMOTE_CONTROL_ID = 66,
        TECHNIC_MEDIUM_HUB = 128
    }
}