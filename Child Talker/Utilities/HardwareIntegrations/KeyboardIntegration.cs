using System.Windows;
using System.Windows.Input;
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
        // TODO find a better way to list all potential trigger events this is inefficient. Learn about RoutedCommands as one possibility
        private enum ControlKeys
        {
            GoBack = Key.S,
            Reverse = Key.Q,
            Select = Key.E,
            GoBack2 = Key.I,
            Reverse2 = Key.J,
            Select2 = Key.L
        }

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
            var k = e.Key;
            switch (k)
            {
                case (Key) ControlKeys.Reverse when !reverseIsHeld:
                case (Key) ControlKeys.Reverse2 when !reverseIsHeld:
                    reverseIsHeld = true;
                    scan.ReverseHoldIntegration();
                    break;
                case (Key) ControlKeys.Select when !selectIsHeld:
                case (Key) ControlKeys.Select2 when !selectIsHeld:
                    selectIsHeld = true;
                    scan.SelectHoldIntegration();
                    break;
                case (Key) ControlKeys.GoBack when !goBackIsHeld:
                case (Key) ControlKeys.GoBack2 when !goBackIsHeld:
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
            var k = e.Key;
            switch (k)
            {
                case (Key) ControlKeys.Reverse:
                case (Key) ControlKeys.Reverse2:
                    scan.ReversePressIntegration();
                    reverseIsHeld = false;
                    break;
                case (Key) ControlKeys.Select:
                case (Key) ControlKeys.Select2:
                    scan.SelectPressIntegration();
                    selectIsHeld = false;
                    break;
                case (Key) ControlKeys.GoBack:
                case (Key) ControlKeys.GoBack2:
                    scan.GoBackPressIntegration();
                    goBackIsHeld = false;
                    break;
                default:
                    return;
            }
        }
    }
}
