using System;
using System.Diagnostics;

namespace bledevice.PoweredUp.Protocol
{
    // ReSharper disable InconsistentNaming
    public class Message
    {
        public enum MessageType : byte
        {
            #region Hub Related

            HubProperties = 0x1,
            HubActions = 0x2,
            HubAlerts = 0x3,
            HubAttachedIO = 0x4,
            GenericErrorMessage = 0x5,
            HardwareNetworkCommands = 0x8,
            FirmwareUpdateGoIntoBootMode = 0x10,
            FirmwareUpdateLockMemory = 0x11,
            FirmwareUpdateLockStatusReport = 0x12,
            FirmwareLockStatus = 0x13,

            #endregion

            #region Port Related

            PortInformationRequest = 0x21,
            PortModeInformationRequest = 0x22,
            PortInputFormatSetupSingle = 0x41,
            PortInputFormatSetupCombined = 0x42,
            PortInformation = 0x43,
            PortModeInformation = 0x44,
            PortValueSingle = 0x45,
            PortValueCombined = 0x46,
            PortInputFormatSingle = 0x47,
            PortInputFormatCombined = 0x48,
            VirtualPortSetup = 0x61,
            PortOutputCommand = 0x81,
            PortOutputCommandFeedback = 0x82

            #endregion
        }

        public enum HubPropertyType : byte
        {
            AdvertisingName = 0x01,
            Button = 0x02,
            FirmwareVersion = 0x03,
            HardwareVersion = 0x04,
            RSSI = 0x05,
            BatteryVoltage = 0x06,
            BatteryType = 0x07,
            ManufactureName = 0x08,
            RadioFirmwareVersion = 0x09,
            LEGOWirelessProtocolVersion = 0x0A,
            SystemTypeID = 0x0B,
            HardwareNetworkID = 0x0C,
            PrimaryMACAddress = 0x0D,
            SecondaryMACAddress = 0x0E,
            HardwareNetworkFamily = 0x0F
        }

        public enum HubPropertyOperation : byte
        {
            Set = 0x01,
            EnableUpdates = 0x02,
            DisableUpdates = 0x03,
            Reset = 0x04,
            RequestUpdate = 0x05,
            Update = 0x06
        }

        public enum HubActionType : byte
        {
            #region Downstream

            SwitchOffHub = 0x01,
            Disconnect = 0x02,
            VCCPortControlOn = 0x03,
            VCCPortControlOff = 0x04,
            ActivateBusyIndication = 0x05,
            ResetBusyIndication = 0x06,
            Shutdown = 0x2f,

            #endregion

            #region Upstream

            HubWillSwitchOff = 0x30,
            HubWillDisconnect = 0x31,
            HubWillGoIntoBootMode = 0x32,

            #endregion
        }

        public enum HubAlertType : byte
        {
            LowVoltage = 0x01,
            HighCurrent = 0x02,
            LowSignalStrength = 0x03,
            Reset = 0x04,
            OverPowerCondition = 0x05
        }

        public enum HubAlertOperation : byte
        {
            EnableUpdates = 0x01,
            DisableUpdates = 0x02,
            RequestUpdate = 0x03,
            Update = 0x04
        }

        public enum HubAttachedIOEvent : byte
        {
            Detached = 0x01,
            Attached = 0x02,
            AttachedVirtual = 0x03
        }

        public enum HubAttachedIOType : ushort
        {
            Motor = 0x0001,
            SystemTrainMotor = 0x0002,
            Button = 0x0005,
            LEDLight = 0x0008,
            Voltage = 0x0014,
            Current = 0x0015,
            PiezoTone = 0x0016,
            RGBLight = 0x0017,
            ExternalTiltSensor = 0x0022,
            MotionSensor = 0x0023,
            VisionSensor = 0x0025,
            ExternalTachoMotor = 0x0026,
            InternalTachoMotor = 0x0027,
            InternalTilt = 0x0028
        }

        public enum ErrorCodes : byte
        {
            ACK = 0x01,
            MACK = 0x02,
            BufferOverflow = 0x03,
            Timeout = 0x04,
            CommandNotRecognized = 0x05,
            InvalidUse = 0x06,
            Overcurrent = 0x07,
            InternalError = 0x08
        }

        private byte[] buffer = {3, 0, 0};

        // TODO: Length escaping/encoding
        public byte Length
        {
            get => buffer[0];
            private set => buffer[0] = value;
        }

        public MessageType Type
        {
            get => (MessageType) buffer[2];
            set => buffer[2] = (byte) value;
        }

        public HubPropertyType PropertyType
        {
            get => (HubPropertyType) buffer[3];
            set => buffer[3] = (byte) value;
        }

        public Span<byte> Payload
        {
            get => new Span<byte>(buffer, 3, buffer.Length - 3);
            set
            {
                if (buffer.Length != value.Length + 3)
                {
                    Array.Resize(ref buffer, value.Length + 3);
                }

                if (buffer.Length > 127)
                {
                    Debug.Assert(false, "Message longer than 127 bytes is not implemented.");
                }

                value.CopyTo(new Span<byte>(buffer, 3, buffer.Length - 3));
                Length = (byte) buffer.Length;
            }
        }

        public Message(MessageType type, byte[] payload)
        {
            Type = type;
            Payload = payload;
        }

        public byte[] ToByteArray()
        {
            return buffer;
        }

        public Message(byte[] buffer)
        {
            this.buffer = (byte[]) buffer.Clone();
        }
    }
}