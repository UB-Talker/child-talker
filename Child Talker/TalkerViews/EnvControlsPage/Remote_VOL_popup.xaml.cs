using System.Windows;
using System.Windows.Controls;
using Child_Talker.Utilities;

namespace Child_Talker.TalkerViews.EnvControlsPage
{
    /// <summary>
    /// Interaction logic for Remote_popup.xaml
    /// </summary>
    public partial class Remote_VOL_popup : SecondaryWindow
    {
        Utilities.Autoscan2 scan;
        EnvControls parent;
        public Remote_VOL_popup(EnvControls _parent)
        {
            InitializeComponent();
            parent = _parent;
            BackButton.Click += ((bSender, bE) => { this.Close(); });
            scan = Utilities.Autoscan2.Instance;
            scan.ClearReturnPointList();
        }

        private void TV_controls(object sender, RoutedEventArgs e)
        {
            parent.TV_Controls(sender, e);
        }
    }
}
