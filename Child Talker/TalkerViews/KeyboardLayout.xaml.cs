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

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for KeyboardLayout.xaml
    /// </summary>
    public partial class KeyboardLayout : Grid
    {
        public static readonly DependencyProperty PhysicalPressProperty = DependencyProperty.Register ( "PhysicalPress", typeof(KeyEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty VirtualPressProperty = DependencyProperty.Register ( "VirtualPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty BackPressProperty = DependencyProperty.Register ( "EnterPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty SpacePressProperty = DependencyProperty.Register ( "SpacePress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty EnterPressProperty = DependencyProperty.Register ( "BackPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );

        public TextUtility util;

        
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
        public event RoutedEventHandler SpacePress;
        public event RoutedEventHandler EscapePress;
        public TextBox textBox;

        public KeyboardLayout( TextBox destination, KeyEventHandler physicalPress, RoutedEventHandler enterPress)
        {
            InitializeComponent();

            util = TextUtility.Instance;
            util.resetAutocorrect();
            this.KeyUp += PhysicalKeyboardKeyUp;

            textBox = destination;
            this.PhysicalPress = physicalPress;
            this.EnterPress = enterPress;
        }
        public KeyboardLayout()
        {
            InitializeComponent();

            util = TextUtility.Instance;
            util.resetAutocorrect();
            this.KeyUp += PhysicalKeyboardKeyUp;

            this.PhysicalPress += PhysicalPressPropertyHandler;
            this.VirtualPress += VirtualPressPropertyHandler;
            this.EnterPress += EnterPressPropertyHandler;
            this.BackPress += BackPressPropertyHandler;
            this.SpacePress += SpacePressPropertyHandler;
        }
        ~KeyboardLayout()
        {
            util?.resetAutocorrect();
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
                    case Key.Space:
                        SpacePress?.Invoke(sender, e);
                        SpacePressDefault();
                        break;
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
                case "SPACE":
                    SpacePress?.Invoke(sender, e);
                    SpacePressDefault();
                    break;
                case "BACK":
                    try
                    {
                        if (defaultBackPress)
                        {
                            if (textBox.Text.Length <= 0)
                            {
                                BackPressWhenEmpty?.Invoke(this, null);
                                break;
                            }
                            else
                            {
                                BackPressDefault();
                            }
                        }
                        BackPress?.Invoke(sender, e);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("There are no characters to delete!");
                        BackPressWhenEmpty?.Invoke(this, null); //probably never called but just to be safe until verified
                    }
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
        /// Gets autofill suggestions after the char c is typed
        /// </summary>
        /// <param name="c"></param>
        public void AddAutocorrect(char c)
        {
            autofill.Children.Clear();
            foreach (Button b in util.getNextSuggestion(c))
            {
                b.Click += AutoCorrectButton;
                autofill.Children.Add(b);
            }
        }



        /// <summary>
        /// Gets autofill suggestions for a given string replaces the autofill bar contents with results
        /// </summary>
        /// <param name="s"></param>
        private void AddAutoFill(string s)
        {
            FormTextBox tb = new FormTextBox();
            autofill.Children.Clear();
            foreach(Button b in util.getNextSuggestionsForString(s))
            {
                b.Click += AutoCorrectButton;
                autofill.Children.Add(b);
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
            /*
             textBox = new TextBox();
            textBox.Background = Brushes.Green;
            textBox.Foreground = Brushes.White;
            textBox.FontSize = 40;
            textBox.Height = 100;
            textBox.Width = 100;
            Grid.SetRow(textBox, 0);
            */
            textBox = OptTextBox;
            return textBox;
        }


        

        private void AutoCorrectButton(object sender, RoutedEventArgs args)
        {
            Button b = sender as Button;

            string s = textBox.Text;
            int i = s.LastIndexOf(' ');
            s = s.Substring(0, i + 1);
            s += b.Text + ' ';
            textBox.Text = s;
            util.resetAutocorrect();
            autofill.Children.Clear();
        }




        private void EnterPressDefault()
        {
            if (!defaultEnterPress) return;
            util.Speak(textBox.Text);
            textBox.Text = "";
        }

        private void SpacePressDefault()
        {
            if (!defaultSpacePress) return; 
            textBox.Text += " ";
            util.resetAutocorrect();
            autofill.Children.Clear();
        }

        private void BackPressDefault()
        {
            if (!defaultBackPress) return;

            textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            AddAutocorrect('_');
            string[] words = textBox.Text.Split(' ');
            AddAutoFill(words.Last<string>());
        }

        private void CharacterPressDefault(string s)
        {
            textBox.Text += s.ToLower();
            AddAutocorrect(s[0]);
        }
    }
}
