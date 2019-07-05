using System;
using System.Collections.Generic;
using System.Windows;
using Child_Talker.Utilities;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class KeyboardPage : TalkerView
    {
        public KeyboardPage()
        {
            InitializeComponent();
            keyboard.textBox = greetingOutput;

        }

        public KeyboardPage(String selectedText)
        {
            InitializeComponent();
            greetingOutput.Text = selectedText;
        }

        /*
         * Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         */
        public sealed override List<DependencyObject> GetParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
                {
                    sidePanel,
                    keyboard.autofill,
                    keyboard.keyboardGrid
                };
            return (parents);
        }

        private void Keyboard_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
