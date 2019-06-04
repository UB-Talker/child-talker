using Child_Talker.TalkerViews;
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
    /// Interaction logic for Remote_popup.xaml
    /// </summary>
    public partial class Remote_VOL_popup : UserControl 
    {
        EnvControls parent;
        public Remote_VOL_popup(EnvControls _parent)
        {
            InitializeComponent();
            parent = _parent;
        }

        private void TV_controls(object sender, RoutedEventArgs e)
        {
            parent.TV_Controls(sender, e);
        }

        private void Close_Button(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

    }
}
