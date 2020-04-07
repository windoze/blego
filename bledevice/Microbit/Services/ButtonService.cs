using System;
using System.Threading.Tasks;
using bledevice.GeneralServices;
using HashtagChris.DotNetBlueZ;
using Serilog;

namespace bledevice.Microbit.Services
{
    public class ButtonService : GattService
    {
        private static readonly Guid BUTTON_A_CHARACTERISTIC = Guid.Parse("e95dda90251d470aa062fa1922dfa9a8");
        private static readonly Guid BUTTON_B_CHARACTERISTIC = Guid.Parse("e95dda91251d470aa062fa1922dfa9a8");

        public enum Button
        {
            A,
            B
        }

        public delegate Task ButtonEventHandlerAsync(Button button, bool pressed);

        private event ButtonEventHandlerAsync Handler;

#pragma warning disable CS8618
        public ButtonService(IGattService1 service) : base(service)
        {
        }

        public async Task Subscribe(ButtonEventHandlerAsync handlerAsync)
        {
            if (Handler == null)
            {
                Handler += handlerAsync;
                await Subscribe(BUTTON_A_CHARACTERISTIC, this.OnButtonEvent);
                await Subscribe(BUTTON_B_CHARACTERISTIC, this.OnButtonEvent);
            }
            else
            {
                Handler += handlerAsync;
            }
        }

#pragma warning disable CS1998
        public async Task Unsubscribe(ButtonEventHandlerAsync handlerAsync)
        {
            if (Handler != null)
            {
                Handler -= handlerAsync;
            }
        }

        private async Task OnButtonEvent(GattCharacteristic sender,
            GattCharacteristicValueEventArgs eventArgs)
        {
            var p = BitConverter.ToBoolean(eventArgs.Value);
            var g = Guid.Parse(await sender.GetUUIDAsync());
            if (g == BUTTON_A_CHARACTERISTIC)
            {
                await Handler(Button.A, p);
            }
            else if (g == BUTTON_B_CHARACTERISTIC)
            {
                await Handler(Button.B, p);
            }
            else
            {
                Log.Warning($"Unknown Button Pressed, UUID is {g}.");
            }
        }
    }
}