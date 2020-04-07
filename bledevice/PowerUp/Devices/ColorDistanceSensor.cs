using System;
using System.Threading.Tasks;
using bledevice.PowerUp.Hubs;
using bledevice.PowerUp.Protocol;

namespace bledevice.PowerUp.Devices
{
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

        public delegate Task ColorHandler(ColorDistanceSensor sender, Color color);

        public event ColorHandler? OnColorChange;

        public delegate Task DistanceHandler(ColorDistanceSensor sender, double distance);

        public event DistanceHandler? OnDistanceChange;
    }
}