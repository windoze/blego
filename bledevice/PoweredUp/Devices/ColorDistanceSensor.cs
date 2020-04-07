using System;
using System.Threading.Tasks;
using bledevice.PoweredUp.Hubs;
using bledevice.PoweredUp.Protocol;

namespace bledevice.PoweredUp.Devices
{
    /// <summary>
    /// Boost Color and Distance Sensor, Part ID 88007
    /// </summary>
    public class ColorDistanceSensor : LPF2Device, ISensor
    {
        private const byte MODE_COLOR = 0x00;
        private const byte MODE_DISTANCE = 0x01;
        private const byte MODE_COLOR_AND_DISTANCE = 0x08;

        public byte DefaultMode => MODE_COLOR_AND_DISTANCE;
        public bool AutoSubscribe => true;

        internal ColorDistanceSensor(LPF2Hub hub, int port) : base(hub, port, LPF2DeviceType.COLOR_DISTANCE_SENSOR)
        {
            Mode = MODE_COLOR_AND_DISTANCE;
        }

        internal override async Task ReceiveMessage(Message msg)
        {
            switch (Mode)
            {
                case MODE_COLOR:
                    if (OnColorChange != null && msg.Payload[1] <= 10)
                    {
                        var color = (Color) msg.Payload[1];
                        await OnColorChange(this, color);
                    }

                    break;
                case MODE_DISTANCE:
                    if (OnDistanceChange != null && msg.Payload[1] <= 10)
                    {
                        var distance = Math.Floor(msg.Payload[1] * 25.4) - 20;
                        await OnDistanceChange(this, distance);
                    }

                    break;
                case MODE_COLOR_AND_DISTANCE:
                    if (OnColorChange != null && msg.Payload[1] <= 10)
                    {
                        var color = (Color) msg.Payload[1];
                        await OnColorChange(this, color);
                    }

                    if (OnDistanceChange != null)
                    {
                        var dist = (double) msg.Payload[2];
                        var partial = (double) msg.Payload[4];
                        if (partial > 0)
                        {
                            dist += 1.0 / partial;
                        }

                        var distance = Math.Floor(dist * 25.4) - 20;
                        await OnDistanceChange(this, distance);
                    }

                    break;
            }
        }

        /// <summary>
        /// Color event handler
        /// </summary>
        /// <param name="sender">The sensor sends the event</param>
        /// <param name="color">The color detected by the sensor</param>
        public delegate Task ColorHandler(ColorDistanceSensor sender, Color color);

        /// <summary>
        /// The color event handler
        /// </summary>
        public event ColorHandler? OnColorChange;

        /// <summary>
        /// Distance event handler
        /// </summary>
        /// <param name="sender">The sensor sends the event</param>
        /// <param name="distance">The distance detected by the sensor</param>
        public delegate Task DistanceHandler(ColorDistanceSensor sender, double distance);

        /// <summary>
        /// The distance event handler
        /// </summary>
        public event DistanceHandler? OnDistanceChange;
    }
}