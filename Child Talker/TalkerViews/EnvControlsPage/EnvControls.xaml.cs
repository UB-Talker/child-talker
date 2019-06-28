using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Child_Talker.Utilities;
using Button = System.Windows.Controls.Button;

namespace Child_Talker.TalkerViews.EnvControlsPage
{
    /// <summary>
    /// Interaction logic for EnvControls.xaml
    /// </summary>
    public partial class EnvControls : TalkerView
    {
        private ConsoleControls cc = new ConsoleControls();
        private SecondaryWindow sw;

        private Remote_VOL_popup vol;
        private Remote_CH_popup ch;
        Utilities.Autoscan2 scan;

        public EnvControls()
        {
            InitializeComponent();
            scan = Utilities.Autoscan2.Instance; //singleton cannot call constructor, call instance
        }
        
        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            vol = new Remote_VOL_popup(this);
            sw = new SecondaryWindow(vol.gridLayout) ;
            vol.BackButton.Click += ((bSender, bE) => { sw.Close(); });
            scan.ClearReturnPointList();
            sw.Show<Button>(vol.gridLayout);
        }
        private void Channel_Click(object sender, RoutedEventArgs e)
        {
            ch = new Remote_CH_popup(this);
            sw = new SecondaryWindow(ch.gridLayout);
            scan.NewListToScanThough<Button>(ch.gridLayout);
            ch.backButton.Click += ((bSender, bE) => { sw.Close(); });
            scan.ClearReturnPointList();
            sw.Show<Button>(ch.gridLayout);
        }
        private void RelayControl(object sender, RoutedEventArgs e)
        {
            string tag = (((Button)sender).Tag).ToString();
            int index = int.Parse(tag); // index is which button to turn on and off
            Debug.Print(index.ToString());
            
            new Thread(() => cc.RelayControl(index))
            {
                IsBackground = true
            }.Start();
        }

        public void TV_Controls(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string command = button.Tag.ToString();
            new Thread( () => cc.remoteControlSend( "STV", command) ){
                IsBackground = true
            }.Start();
            
        }
        public override List<DependencyObject> GetParents()
        {
            List<DependencyObject> parents = new List<DependencyObject> { row1, row2 };
            return(parents);
        }

        private void BackButton(object sender, RoutedEventArgs e)
        {
            OpenPreviousView(sender, e);
        }
    }
}
