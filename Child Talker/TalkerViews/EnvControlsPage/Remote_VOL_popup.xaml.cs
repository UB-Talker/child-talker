using System.Windows;
using System.Windows.Controls;

namespace Child_Talker.TalkerViews.EnvControlsPage
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
    }
}
