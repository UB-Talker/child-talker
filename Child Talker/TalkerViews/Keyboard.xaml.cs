using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : TalkerView
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        public Keyboard()
        {
            InitializeComponent();
            sb = new StringBuild();
            this.KeyDown += physicalKeyboard;
            //zeroKey.Click += Button_Click; //add to routedEventhandler
            //zeroKey.Click -= Button_Click; //remove from routedEventhandler
        }
        public Keyboard(String selectedText)
        {
            InitializeComponent();
            greetingOutput.Text = selectedText;
            greetingOutput.Text = selectedText;
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
          
            switch (s)
            {
                case "SPACE":
                    greetingOutput.Text += " ";
                    break;
                case "BACK":
                    try {
                        greetingOutput.Text = greetingOutput.Text.Substring(0, greetingOutput.Text.Length - 1);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("There are no characters to delete!");
                    }
                    break;
                case "ENTER":
                    EnterPress();
                    break;
                default:
                    greetingOutput.Text += s;
                    break;

            }
        }

        private void EnterPress()
        {
            synth.SpeakAsync(greetingOutput.Text);
            greetingOutput.Text = "";
            return;
        }
    }
}
