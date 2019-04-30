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
        private List<Button> thisButtons = new List<Button>();
        private List<Button> currentObject = new List<Button>(); //buttons being autoscanned

        Remote_VOL_popup vol;
        Remote_CH_popup ch;

        public EnvControls()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //GetLogicalChildCollection(this, thisButtons);
            //currentObject = thisButtons;
            //runTimer(); //initializes timer
        
            //this.Closed += terminate_program;
            vol = new Remote_VOL_popup(this);
            ch = new Remote_CH_popup(this);
        }
        
        // adds all child objects of type T to logicalCollection
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }
        
        private void Volume_Click(object sender, RoutedEventArgs e)
        {

            vol.Show();
            //List<Button> temp = new List<Button>();
            //GetLogicalChildCollection(vol, temp);
            //currentObject = temp;
            vol.Closing += popup_closed;
            //vol.KeyDown += Key_down;
       
            if(getWindow().isScanning())
            {
                Autoscan sc = getWindow().toggleAutoscan(); //if autoscan is on, stop scanning
                sc.partialAutoscan<DependencyObject>(vol.gridLayout, vol);
            }
        }
        private void Channel_Click(object sender, RoutedEventArgs e)
        {
            ch.Show();
            List<Button> temp = new List<Button>();
            GetLogicalChildCollection(ch, temp);
            currentObject = temp;
            ch.Closing += popup_closed;
            //ch.KeyDown += Key_down;

            if (getWindow().isScanning())
            {
                Autoscan sc = getWindow().toggleAutoscan(); //if autoscan is on, stop scanning
                sc.partialAutoscan<DependencyObject>(ch.gridLayout, ch);
            }
        }
        private void popup_closed(object sender, CancelEventArgs e)
        {
            currentObject = thisButtons;
            //vol.KeyDown -= Key_down;
            //ch.KeyDown -= Key_down;
        

            // Cancel Window closing 
            e.Cancel = true;
            // Hide Window instead
            vol.Hide();
            ch.Hide();
            if(getWindow().isScanning())
            {
                Autoscan sc = getWindow().toggleAutoscan(); //stops autoscan if its on
                sc.startAutoscan<DependencyObject>(getWindow());
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
            List<DependencyObject> parents = new List<DependencyObject>();
            parents.Add(row1);
            parents.Add(row2);
            return(parents);
        }

 
        private void terminate_program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
