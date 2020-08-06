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
        /// <summary>
        /// Boolean stating if any settings have been changed. Used in the xaml to enable/disable the "apply, revert" buttons
        /// </summary>
        public bool ChangesMade
        {
            get => applyChanges.IsEnabled;
            private set
            {
                revertChanges.IsEnabled = value;
                applyChanges.IsEnabled = value;
            }
        }

        private void ColorSchemeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is ColorSchemeButton btn)) return;
            Color btnBack = ((SolidColorBrush) btn.HighlightBackground).Color;

            Properties.Settings.Default.Background_Base = ((SolidColorBrush)btn.Background).Color;
            Properties.Settings.Default.Background_Highlighted = ((SolidColorBrush)btn.HighlightBackground).Color; 
            Properties.Settings.Default.Foreground_Base = ((SolidColorBrush)btn.Foreground).Color;
            Properties.Settings.Default.Foreground_Highlighted = ((SolidColorBrush)btn.HighlightForeground).Color;
            Properties.Settings.Default.BorderColor_Base = ((SolidColorBrush)btn.BorderBrush).Color;
            Properties.Settings.Default.BorderColor_Highlighted = ((SolidColorBrush)btn.HighlightBorder).Color;

            int r = (btnBack.R /4)*3;
            int g = (btnBack.G /4)*3;
            int b = (btnBack.B /4)*3;
            btnBack = Color.FromArgb(255, (byte) r, (byte) g, (byte) b);
            Properties.Settings.Default.Background_MouseHover = btnBack;

            Properties.Settings.Default.Page_Background = ((SolidColorBrush)btn.PageBackground).Color;
            

                //Properties.Settings.Default.Background_EventOuter = ((SolidColorBrush)btn.BorderBrush).Color;
            //Properties.Settings.Default.= ((SolidColorBrush)btn.BorderBrush).Color;
            //Properties.Settings.Default.= ((SolidColorBrush)btn.BorderBrush).Color;


            ChangesMade = true;
        }

        private void AutoscanSpeedButtons_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;
            double scanSpeed = Convert.ToDouble(btn.Tag);
            Properties.AutoscanSettings.Default.scanSpeed = scanSpeed;
            //Autoscan2.Instance.UpdateScanTimerInterval();
            ChangesMade = true;
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
            Properties.AutoscanSettings.Default.Reload();
            //Autoscan2.Instance.UpdateScanTimerInterval();
            ChangesMade = false;
        }

        private void ApplyChanges_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Properties.AutoscanSettings.Default.Save();
            ChangesMade = false;
        }

        private void ConnectToRoku_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
