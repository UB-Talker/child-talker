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

        private Remote_VOL_popup vol;
        private Remote_CH_popup ch;
        Utilities.Autoscan scan;

        public EnvControls()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //GetLogicalChildCollection(this, thisButtons);
            //currentObject = thisButtons;
            //runTimer(); //initializes timer

            //this.Closed += terminate_program;

            scan = Utilities.Autoscan.Instance; //singleton cannot call constructor, call instance
        }
        
        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            vol = new Remote_VOL_popup(this);
            sw = new SecondaryWindow(Window.GetWindow(this), vol.gridLayout) ;
            vol.backButton.Click += ((object bSender, RoutedEventArgs bE) => { Window.GetWindow(this)?.Close(); });
            scan.ClearParentPanel();
            sw.Show();
        }
        private void Channel_Click(object sender, RoutedEventArgs e)
        {
            ch = new Remote_CH_popup(this);
            sw = new SecondaryWindow(Window.GetWindow(this), ch.gridLayout);
            scan.StartAutoscan<Button>(ch.gridLayout);
            ch.backButton.Click += ((object bSender, RoutedEventArgs bE) => { Window.GetWindow(this)?.Close(); });
            scan.ClearParentPanel();
            sw.Show();
        }
        private void RelayControl(object sender, RoutedEventArgs e)
        {
            string _tag = (((Button)sender).Tag).ToString();
            int _t = int.Parse(_tag);
            Debug.Print(_t.ToString());
            
            new Thread(() => cc.RelayControl(_t))
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
        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject> { row1, row2 };
            return(parents);
        }

        private void backButton(object sender, RoutedEventArgs e)
        {
            openPreviousView(sender, e);
        }
    }
}
