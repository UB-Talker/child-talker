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
        }

        /*
         * Constructor for the keyboard. This passes a string to set the text for the Keyboard. It is currently used by the History page
         * to send a selected piece of text to the keyboard.
         */ 
        public Keyboard(string text)
        {
            InitializeComponent();
            TextArea.Text = text;
        }
    }
}
