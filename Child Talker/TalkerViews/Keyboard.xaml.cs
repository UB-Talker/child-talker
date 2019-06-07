using System;
using System.Collections.Generic;
using System.Windows;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : TalkerView
    {
        public Keyboard()
        {
            InitializeComponent();
            keyboard.textBox = greetingOutput; //haven't figured out a better way to do this yet
        }

        public Keyboard(String selectedText)
        {
            InitializeComponent();
            greetingOutput.Text = selectedText;
            greetingOutput.Text = selectedText;
        }

        /*
         * Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         */
        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
                {
                    sidePanel,
                    keyboard.autofill,
                    keyboard.keyboardGrid
                };
            return (parents);
        }
    }
}
