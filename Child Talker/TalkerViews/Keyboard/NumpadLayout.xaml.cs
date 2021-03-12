using Child_Talker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FormTextBox = System.Windows.Forms.TextBox;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Child_Talker.TalkerButton;
using Button = Child_Talker.TalkerButton.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using TextBox = System.Windows.Controls.TextBox;

namespace Child_Talker.TalkerViews.Keyboard
{
    /// <summary>
    /// Interaction logic for NumpadLayout.xaml
    /// </summary>
    public partial class NumpadLayout : Grid
    {
        public static readonly DependencyProperty PhysicalPressProperty = DependencyProperty.Register ( "PhysicalPress", typeof(KeyEventHandler), typeof(NumpadLayout), new PropertyMetadata() );
        public static readonly DependencyProperty VirtualPressProperty = DependencyProperty.Register ( "VirtualPress", typeof(RoutedEventHandler), typeof(NumpadLayout), new PropertyMetadata() );
        public static readonly DependencyProperty BackPressProperty = DependencyProperty.Register ( "EnterPress", typeof(RoutedEventHandler), typeof(NumpadLayout), new PropertyMetadata() );
        public static readonly DependencyProperty SpacePressProperty = DependencyProperty.Register ( "SpacePress", typeof(RoutedEventHandler), typeof(NumpadLayout), new PropertyMetadata() );
        public static readonly DependencyProperty EnterPressProperty = DependencyProperty.Register ( "BackPress", typeof(RoutedEventHandler), typeof(NumpadLayout), new PropertyMetadata() );


        
        // event
        private KeyEventHandler PhysicalPressPropertyHandler
        {
            get => (KeyEventHandler)GetValue(PhysicalPressProperty); 
            set => SetValue(PhysicalPressProperty, value); 
        }
        private RoutedEventHandler VirtualPressPropertyHandler
        {
            get => (RoutedEventHandler)GetValue(VirtualPressProperty); 
            set => SetValue(VirtualPressProperty, value); 
        }
        private RoutedEventHandler SpacePressPropertyHandler
        {
            get => (RoutedEventHandler)GetValue(SpacePressProperty); 
            set => SetValue(SpacePressProperty, value); 
        }
        private RoutedEventHandler EnterPressPropertyHandler
        {
            set => SetValue(EnterPressProperty, value); 
            get => (RoutedEventHandler)GetValue(EnterPressProperty); 
        }
        private RoutedEventHandler BackPressPropertyHandler
        {
            get => (RoutedEventHandler)GetValue(BackPressProperty); 
            set => SetValue(BackPressProperty, value); 
        }


        public bool defaultVirtualPress = true;
        public bool defaultEnterPress = true;
        public bool defaultBackPress = true;
        public bool defaultSpacePress = true;
        public event KeyEventHandler PhysicalPress;
        public event RoutedEventHandler VirtualPress;
        public event RoutedEventHandler EnterPress;
        public event RoutedEventHandler BackPress;
        /// <summary>
        /// this will only trigger when default back behavior is enabled and the are no more characters to delete
        /// </summary>
        public event RoutedEventHandler BackPressWhenEmpty;
        public event RoutedEventHandler EscapePress;
        public TextBox textBox;

        public NumpadLayout( TextBox destination, KeyEventHandler physicalPress, RoutedEventHandler enterPress)
        {
            InitializeComponent();

            this.KeyUp += PhysicalKeyboardKeyUp;

            textBox = destination;
            this.PhysicalPress = physicalPress;
            this.EnterPress = enterPress;
        }
        public NumpadLayout()
        {
            InitializeComponent();

            this.KeyUp += PhysicalKeyboardKeyUp;

            this.PhysicalPress += PhysicalPressPropertyHandler;
            this.VirtualPress += VirtualPressPropertyHandler;
            this.EnterPress += EnterPressPropertyHandler;
            this.BackPress += BackPressPropertyHandler;
        }
        ~NumpadLayout()
        {
            this.KeyUp -= PhysicalKeyboardKeyUp;
        }

        private void PhysicalKeyboardKeyUp(object sender, KeyEventArgs e)
        {
            PhysicalPress?.Invoke(this, e);
            //integers
            if (34 <= ((int) e.Key) && ((int) e.Key) < 44)
            { 
                string ch = ((int)(e.Key-34)).ToString();
                CharacterPressDefault(ch);
            }
            //characters
            else if (44 <= ((int)e.Key) &&  ((int)e.Key)< 70)
            {
                string ch = ((Char)(e.Key+21)).ToString();
                CharacterPressDefault(ch);
            }
            else //special keys
            {
                switch (e.Key)
                {
                    case Key.Back:
                        BackPressDefault();
                        break;
                    case Key.Enter:
                        EnterPress?.Invoke(sender, e);
                        EnterPressDefault();
                        break;
                    case Key.Escape:
                        EscapePress?.Invoke(sender, e);
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VirtualPress?.Invoke(sender, e);

            string s = (sender as Button).Text;

            switch (s)
            {
                case "BACKSPACE" :
                    try
                    {
                        if (defaultBackPress) {
                            if (textBox.Text.Length <= 0) {
                                BackPressWhenEmpty?.Invoke(this, null);
                                break;
                            } else {
                                BackPressDefault();
                            }
                        }
                        BackPress?.Invoke(sender, e);
                    }
                    catch (ArgumentOutOfRangeException) { BackPressWhenEmpty?.Invoke(this, null);  } //probably never called but just to be safe until verified
                    break;
                case "ENTER":
                    EnterPress?.Invoke(sender, e);
                    EnterPressDefault();
                    break;
                default:
                    CharacterPressDefault(s);
                    break;
            }
        }

        /// <summary>
        /// creates a textbox to be used by keyboard
        /// textBox can be manually assigned as well if desired
        /// </summary>
        /// <returns></returns>
        public TextBox AddTextBox()
        {
            MainGrid.RowDefinitions[0].Height = new GridLength(125);
            textBox = OptTextBox;
            return textBox;
        }

        private void EnterPressDefault()
        {
            if (!defaultEnterPress) return;
            textBox.Text = "";
        }

        private void BackPressDefault()
        {
            if (!defaultBackPress) return;

            textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            string[] words = textBox.Text.Split(' ');
        }

        private void CharacterPressDefault(string s)
        {
            textBox.Text += s.ToLower();
        }
    }
}
