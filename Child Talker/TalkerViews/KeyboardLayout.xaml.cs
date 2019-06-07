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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for KeyboardLayout.xaml
    /// </summary>
    public partial class KeyboardLayout : TalkerView
    {
        public static readonly DependencyProperty PhysicalPressProperty = DependencyProperty.Register ( "PhysicalPress", typeof(KeyEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty VirtualPressProperty = DependencyProperty.Register ( "VirtualPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty BackPressProperty = DependencyProperty.Register ( "EnterPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty SpacePressProperty = DependencyProperty.Register ( "SpacePress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );
        public static readonly DependencyProperty EnterPressProperty = DependencyProperty.Register ( "BackPress", typeof(RoutedEventHandler), typeof(KeyboardLayout), new PropertyMetadata() );

        public TextUtility util;

        
        // event
        private KeyEventHandler PhysicalPressHandler
        {
            get => (KeyEventHandler)GetValue(PhysicalPressProperty); 
            set => SetValue(PhysicalPressProperty, value); 
        }
        private RoutedEventHandler VirtualPressHandler
        {
            get => (RoutedEventHandler)GetValue(VirtualPressProperty); 
            set => SetValue(VirtualPressProperty, value); 
        }
        private RoutedEventHandler SpacePressHandler
        {
            get => (RoutedEventHandler)GetValue(SpacePressProperty); 
            set => SetValue(SpacePressProperty, value); 
        }
        private RoutedEventHandler EnterPressHandler
        {
            set => SetValue(EnterPressProperty, value); 
            get => (RoutedEventHandler)GetValue(EnterPressProperty); 
        }
        private RoutedEventHandler BackPressHandler
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
        public event RoutedEventHandler SpacePress;
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

            this.PhysicalPress += PhysicalPressHandler;
            this.VirtualPress += VirtualPressHandler;
            this.EnterPress += EnterPressHandler;
            this.BackPress += BackPressHandler;
            this.SpacePress += SpacePressHandler;
        }

        private void PhysicalKeyboardKeyUp(object sender, KeyEventArgs e)
        {
            PhysicalPress?.Invoke(this, e);
            //((Key)e.Key).
            if (34 <= ((int) e.Key) && ((int) e.Key) < 44)
            {
                string ch = ((int)(e.Key-34)).ToString();
                CharacterPressDefault(ch);
            }
            else if (44 <= ((int)e.Key) &&  ((int)e.Key)< 70)
            {
                string ch = ((Char)(e.Key+21)).ToString();
                CharacterPressDefault(ch);
            }
            else
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
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VirtualPress?.Invoke(sender, e);

            string s = (sender as TlkrBTN).Tag.ToString();

            switch (s)
            {
                case "SPACE":
                    SpacePress?.Invoke(sender, e);
                    SpacePressDefault();
                    break;
                case "BACK":
                    BackPress?.Invoke(sender, e);
                    try
                    {
                        BackPressDefault();
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("There are no characters to delete!");
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

        /*
         * Gets autofill suggestions after the char c is typed
         */
        public void addAutocorrect(char c)
        {
            autofill.Children.Clear();
            foreach (TlkrBTN b in util.getNextSuggestion(c))
            {
                b.Click += AutoCorrectButton;
                autofill.Children.Add(b);
            }
        }

        /*
         * Gets autofill suggestions for a given string
         */
        private void addAutoFill(string s)
        {
            autofill.Children.Clear();
            foreach(TlkrBTN b in util.getNextSuggestionsForString(s))
            {
                b.Click += AutoCorrectButton;
                autofill.Children.Add(b);
            }
        }


        /* Used for autoscan, please update if xaml is changed
        * Must return the panels to iterate through when autoscan is first initialized on this page
        */

        public List<DependencyObject> getRows()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                row0,
                row1,
                row2,
                row3,
                row4
            };
            return (parents);
        }

        override public void update()
        {
            util.resetAutocorrect();
        }

        private void AutoCorrectButton(object sender, RoutedEventArgs args)
        {
            TlkrBTN b = sender as TlkrBTN;
            string s = textBox.Text;
            int i = s.LastIndexOf(' ');
            s = s.Substring(0, i + 1);
            s += (string)b.Tag + ' ';
            textBox.Text = s;
            util.resetAutocorrect();
            autofill.Children.Clear();
        }


        private void EnterPressDefault()
        {
            if (!defaultEnterPress) return;
            util.speak(textBox.Text);
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
            if (!defaultBackPress && textBox.Text.Length > 0) { return; }
            textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            addAutocorrect('_');
            string[] words = textBox.Text.Split(' ');
            addAutoFill(words.Last<string>());
        }

        private void CharacterPressDefault(string s)
        {
            textBox.Text += s.ToLower();
            addAutocorrect(s[0]);
        }
    }
}
