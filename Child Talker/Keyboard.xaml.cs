using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : Window
    {
        StringBuild sb;
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        public Keyboard()
        {
            sb = new StringBuild();
            InitializeComponent();
            this.KeyDown += physicalKeyboard;
            //zeroKey.Click += Button_Click; //add to routedEventhandler
            //zeroKey.Click -= Button_Click; //remove from routedEventhandler
        }

        private void physicalKeyboard(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            if (k == Key.Enter)
            {
                EnterPress();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = (sender as Button).Content.ToString();
           
            if (s.Equals("ENTER"))
            {
                EnterPress();
                return;
            }
            else if (s.Equals("SPACE"))
            {
                greetingOutput.Text += " ";          
                return;
            }
            else if (s.Equals("BACK"))
            {
                try
                {
                    greetingOutput.Text = greetingOutput.Text.Substring(0, greetingOutput.Text.Length - 1); //deletes a character
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    Console.WriteLine("There are no characters to delete!");
                }
                Console.WriteLine(sb.text);
                return;
            }

            sb.text += s;
            greetingOutput.Text = sb.text;
            Console.WriteLine(sb.text);
        }

        private void EnterPress()
        {
            sb.text = greetingOutput.Text;
            synth.SpeakAsync(sb.text);
            sb.text = "";
            greetingOutput.Text = "";
            return;
        }
    }

    
}
