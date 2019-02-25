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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string s = (sender as Button).Content.ToString();
           
            if (s.Equals("ENTER"))
            {
                synth.SpeakAsync(sb.text);
                sb.text = "";
                return;
            }
            else if (s.Equals("SPACE"))
            {
                sb.text += " ";          
                return;
            }
            else if (s.Equals("BACK"))
            {
                try
                {
                    sb.text = sb.text.Substring(0, sb.text.Length - 1); //deletes a character
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    Console.WriteLine("There are no characters to delete!");
                }
                Console.WriteLine(sb.text);
                return;
            }

            sb.text += s;
            greetingOutput.Text = "Hello";
            Console.WriteLine(sb.text);
        }
    }
}
