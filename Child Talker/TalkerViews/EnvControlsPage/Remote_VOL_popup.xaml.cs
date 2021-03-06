using System.Windows;
using System.Windows.Controls;
using Child_Talker.Utilities;
using Child_Talker.Utilities.Autoscan;

namespace Child_Talker.TalkerViews.EnvControlsPage
{
    /// <summary>
    /// Interaction logic for Remote_popup.xaml
    /// </summary>
    public partial class Remote_VOL_popup : SecondaryWindow
    {
        private Autoscan2 scan;
        private EnvControls parent;

        public Remote_VOL_popup(EnvControls _parent)
        {
            InitializeComponent();
            parent = _parent;
            BackButton.Click += ((bSender, bE) => { this.Close(); });
            scan = Autoscan2.Instance;
            scan.ClearReturnPointList();
        }

        private void TV_controls(object sender, RoutedEventArgs e)
        {
            parent.TV_Controls(sender, e);
        }
    }
}
