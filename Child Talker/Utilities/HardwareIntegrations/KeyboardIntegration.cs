using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Child_Talker.Utilities.Autoscan;

namespace Child_Talker.Utilities.HardwareIntegrations
{

    public class KeyboardIntegration
    {
        public static KeyboardIntegration _instance;
        public static KeyboardIntegration Instance => _instance ?? (_instance = new KeyboardIntegration());
        private Autoscan2 scan = Autoscan2.Instance;

        static KeyboardIntegration() { }

        public void ApplyIntegration(Window newWindow)
        {
            newWindow.Dispatcher?.Invoke(() =>
            {
                newWindow.KeyUp += KeyUp;
                newWindow.KeyDown += KeyDown;
            });
        }

        /// List of Keyboard Keys that are used for autoscan
        private readonly Dictionary<Key, Autoscan2.ControlOptions> controlKeys =
            new Dictionary<Key, Autoscan2.ControlOptions>
            {
                {Key.S, Autoscan2.ControlOptions.GoBack},
                {Key.Q, Autoscan2.ControlOptions.Reverse},
                {Key.E, Autoscan2.ControlOptions.Select},
                {Key.I, Autoscan2.ControlOptions.GoBack},
                {Key.J, Autoscan2.ControlOptions.Reverse},
                {Key.L, Autoscan2.ControlOptions.Select}
            };
        


        /// keeps hold event from happening repeatedly
        private bool reverseIsHeld = false;

        /// keeps hold event from happening repeatedly
        private bool selectIsHeld = false;

        /// keeps hold event from happening repeatedly
        private bool goBackIsHeld = false;

        /// triggers KeyHold behavior
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (scan.TimerMode == Autoscan2.TimerModes.Off) return;
            Autoscan2.ControlOptions k;
            try
            {
                k = controlKeys[e.Key];
            }
            catch
            {
                return;
            }

            switch (k)
            {
                case Autoscan2.ControlOptions.Reverse when !reverseIsHeld:
                    reverseIsHeld = true;
                    scan.ReverseHoldIntegration();
                    break;
                case Autoscan2.ControlOptions.Select when !selectIsHeld:
                    selectIsHeld = true;
                    scan.SelectHoldIntegration();
                    break;
                case Autoscan2.ControlOptions.GoBack when !goBackIsHeld:
                    goBackIsHeld = true;
                    scan.GoBackHoldIntegration();
                    break;
                default:
                    return;
            }
        }

        // triggers KeyPress behavior
        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (scan.TimerMode == Autoscan2.TimerModes.Off) return;
            Autoscan2.ControlOptions k;
            try
            {
                k = controlKeys[e.Key];
            }
            catch
            {
                return;
            }
            switch (k)
            {
                case Autoscan2.ControlOptions.Reverse:
                    scan.ReversePressIntegration();
                    reverseIsHeld = false;
                    break;
                case Autoscan2.ControlOptions.Select:
                    scan.SelectPressIntegration();
                    selectIsHeld = false;
                    break;
                case Autoscan2.ControlOptions.GoBack:
                    scan.GoBackPressIntegration();
                    goBackIsHeld = false;
                    break;
                default:
                    return;
            }
        }
    }
}
