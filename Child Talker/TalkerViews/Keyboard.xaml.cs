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
        private Autoscan scan;

        public Keyboard()
        {
            InitializeComponent();
            scan = Autoscan.instance;
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
