﻿using System;
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
        StringBuild sb;
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
            sb = new StringBuild();
            sb.text = selectedText;
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

            if (s.Equals("ENTER"))
            {
                EnterPress();
                return;
            }
            else if (s.Equals("SPACE"))
            {
                sb.text += " ";
                greetingOutput.Text = sb.text;
                return;
            }
            else if (s.Equals("BACK"))
            {
                try
                {
                    sb.text = sb.text.Substring(0, sb.text.Length - 1);
                    greetingOutput.Text = sb.text; //deletes a character
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