using System.Windows;
using EnvControls = Child_Talker.TalkerViews.EnvControlsPage.EnvControls;

namespace Child_Talker.TalkerViews.SettingPage
{
    /// <summary>
    /// Interaction logic for Remote_popup.xaml
    /// </summary>
    public partial class ColorSchemes : SecondaryWindow
    {
        EnvControls parent;
        public ColorSchemes(EnvControls _parent)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            parent = _parent;
        }
    }
}
