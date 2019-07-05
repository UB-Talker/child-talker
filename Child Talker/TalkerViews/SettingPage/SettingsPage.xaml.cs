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


namespace Child_Talker.TalkerViews.SettingPage
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : TalkerView
    {
        public SettingsPage()
        {
            InitializeComponent();        
        }


        /* Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         * Currently goes between the phrase stack and side menu
         */
        public override List<DependencyObject> GetParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                sidePanel, SettingsList
            };
            return (parents);
        }

        public bool ChangesMade { get; private set; } = false;
        private void ColorSchemeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is ColorSchemeButton btn)) return;
            Properties.Settings.Default.BackgroundBase = ((SolidColorBrush)btn.Background).Color;
            Properties.Settings.Default.BackgroundHighlighted= ((SolidColorBrush)btn.HighlightBackground).Color; 
            Properties.Settings.Default.ForegroundBase= ((SolidColorBrush)btn.Foreground).Color;
            Properties.Settings.Default.ForegroundHighlighted= ((SolidColorBrush)btn.HighlightForeground).Color;
            Properties.Settings.Default.BorderColorBase= ((SolidColorBrush)btn.BorderBrush).Color;
            ChangesMade = true;
            revertChanges.IsEnabled = ChangesMade;
            applyChanges.IsEnabled = ChangesMade;

        }

        private void AutoscanSpeedButtons_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;
            double scanSpeed = Convert.ToDouble(btn.Tag);
            Properties.Settings.Default.AutoscanTimerSpeed = scanSpeed;
            Autoscan2.Instance.UpdateScanTimerInterval();
            ChangesMade = true;
            revertChanges.IsEnabled = ChangesMade;
            applyChanges.IsEnabled = ChangesMade;
        }

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            RevertChanges_OnClick(sender, e);
            backToHome(sender, e);
        }

        private void RevertChanges_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ChangesMade) return;

            Properties.Settings.Default.Reload();
            Autoscan2.Instance.UpdateScanTimerInterval();
            ChangesMade = false;
            revertChanges.IsEnabled = ChangesMade;
            applyChanges.IsEnabled = ChangesMade;
        }

        private void ApplyChanges_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            ChangesMade = false;
            revertChanges.IsEnabled = ChangesMade;
            applyChanges.IsEnabled = ChangesMade;
        }
    }
}
