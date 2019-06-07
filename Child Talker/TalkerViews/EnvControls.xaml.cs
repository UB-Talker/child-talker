using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Timer = System.Timers.Timer;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Button = System.Windows.Controls.Button;
using System;
using System.ComponentModel;
using System.Threading;
using Child_Talker;
using Child_Talker.TalkerViews;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for EnvControls.xaml
    /// </summary>
    public partial class EnvControls : TalkerView
    {
        private ConsoleControls cc = new ConsoleControls();
        private SecondaryWindow sw;

        Remote_VOL_popup vol;
        Remote_CH_popup ch;
        Utilities.Autoscan scan;

        public EnvControls()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //GetLogicalChildCollection(this, thisButtons);
            //currentObject = thisButtons;
            //runTimer(); //initializes timer

            //this.Closed += terminate_program;
            sw = new SecondaryWindow(this.getWindow());
            sw.Closing += popup_closed;
            sw.Hide();

            scan = Utilities.Autoscan.instance; //singleton cannot call constructor, call instance
            vol = new Remote_VOL_popup(this);
            ch = new Remote_CH_popup(this);
        }
        
        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            sw.DataContext = vol;
            sw.Show();
            if(getWindow().isScanning())
            {
                scan.updateActiveWindow(sw);
                scan.startAutoscan<Button>(vol.gridLayout);
            }
        }
        private void Channel_Click(object sender, RoutedEventArgs e)
        {
            sw.DataContext = ch;
            sw.Show();
            if (getWindow().isScanning())
            {
                scan.updateActiveWindow(sw);
                scan.startAutoscan( Utilities.Autoscan.generateObjectList<Button>( ch.gridLayout, new List<DependencyObject>() ) );
            }
        }
        private void popup_closed(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            sw.Hide();
            if(getWindow().isScanning())
            {
                scan.updateActiveWindow(this.getWindow());
                scan.startAutoscan(this.getParents());
            }
        }

        private void relayControl(object sender, RoutedEventArgs e)
        {
          
            string _tag = (((Button)sender).Tag).ToString();
            int _t = int.Parse(_tag);
            Debug.Print(_t.ToString());
            
            new Thread(() => cc.relayControl(_t))
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
        override public List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject> { row1, row2 };
            return(parents);
        }

        private void backButton(object sender, RoutedEventArgs e)
        {
            sw.Closing -= popup_closed;
            sw.Close();

            openPreviousView(sender, e);
        }
    }
}
