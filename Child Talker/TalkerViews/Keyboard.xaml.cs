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
using System.Windows.Shapes;
using Child_Talker.Utilities;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : TalkerView
    {
        private TextUtility util;


        public Keyboard()
        {
            InitializeComponent();
            this.KeyDown += physicalKeyboard;
            util = TextUtility.Instance;
            util.resetAutocorrect();

            //zeroKey.Click += Button_Click; //add to routedEventhandler
            //zeroKey.Click -= Button_Click; //remove from routedEventhandler
        }


        public Keyboard(String selectedText)
        {
            InitializeComponent();
            greetingOutput.Text = selectedText;
            greetingOutput.Text = selectedText;
            this.KeyDown += physicalKeyboard;
            util = TextUtility.Instance;
            util.resetAutocorrect();
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
                    util.resetAutocorrect();
                    autofill.Children.Clear();
                    break;
                case "BACK":
                    try {
                        greetingOutput.Text = greetingOutput.Text.Substring(0, greetingOutput.Text.Length - 1);
                        addAutocorrect('_');
                        string[] words = greetingOutput.Text.Split(' ');
                        addAutoFill(words.Last<string>());
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
                    addAutocorrect(s[0]);
                    break;

            }
        }

        private void EnterPress()
        {
            util.speak(greetingOutput.Text);
            greetingOutput.Text = "";
            return;
        }


        /*
         * Gets autofill suggestions after the char c is typed
         */
        private void addAutocorrect(char c)
        {
            autofill.Children.Clear();
            foreach (Button b in util.getNextSuggestion(c))
            {
                b.Click += autoCorrectButton;
                Border border = new Border();
                border.Child = b;
                autofill.Children.Add(border);
            }
        }

        /*
         * Gets autofill suggestions for a given string
         */
        private void addAutoFill(string s)
        {
            autofill.Children.Clear();
            foreach(Button b in util.getNextSuggestionsForString(s))
            {
                b.Click += autoCorrectButton;
                Border border = new Border();
                border.Child = b;
                autofill.Children.Add(border);
            }
        }


        /*
         * Used to update the autocorrect when the page is loaded to ensure that 
         * it is working properly.
         */
        override public void update()
        {
            util.resetAutocorrect();
        }


        private void autoCorrectButton(object sender, RoutedEventArgs args)
        {
            Button b = sender as Button;
            string s = greetingOutput.Text;
            int i = s.LastIndexOf(' ');
            s = s.Substring(0, i + 1);
            s += (string)b.Content + ' ';
            greetingOutput.Text = s;
            util.resetAutocorrect();
            autofill.Children.Clear();
        }
    }
}
