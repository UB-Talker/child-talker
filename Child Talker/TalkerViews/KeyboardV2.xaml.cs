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
    /// Interaction logic for UserControl1.xaml
    /// 
    /// 
    /// This is Sam's attempt at designing a keyboard. Based off of Ian's original design. I tried to make the spacing of the keys
    /// look similar to that of a QWERTY keyboard.
    /// 
    /// </summary>
    public partial class KeyboardV2 : TalkerView
    {
        private string text;

        public KeyboardV2()
        {
            InitializeComponent();
        }

    }
}
