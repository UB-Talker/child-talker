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
        private List<Button> currentButtons = new List<Button>(); //buttons being autoscanned

        private Timer aTimer;
        private Button highlightedButton;
        private int indexHighlighted;

        Remote_VOL_popup vol;
        Remote_CH_popup ch;

        public EnvControls()
        {
            InitializeComponent();
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GetLogicalChildCollection(this, thisButtons);
            currentButtons = thisButtons;
            runTimer(); //initializes timer

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
            List<Button> temp = new List<Button>();
            GetLogicalChildCollection(vol, temp);
            currentButtons = temp;
            vol.Closing += popup_closed;
            vol.KeyDown += Key_down;
            indexHighlighted = 0;
        }
        private void Channel_Click(object sender, RoutedEventArgs e)
        {
            ch.Show();
            List<Button> temp = new List<Button>();
            GetLogicalChildCollection(ch, temp);
            currentButtons = temp;
            ch.Closing += popup_closed;
            ch.KeyDown += Key_down;
            indexHighlighted = 0;
        }
        private void popup_closed(object sender, CancelEventArgs e)
        {
            currentButtons = thisButtons;
            vol.KeyDown -= Key_down;
            ch.KeyDown -= Key_down;
            indexHighlighted = 0;

            // Cancel Window closing 
            e.Cancel = true;
            // Hide Window instead
            vol.Hide();
            ch.Hide();
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

        //using System.Timers
        public void runTimer()
        {
            aTimer = new Timer(1000); //

            aTimer.Elapsed += new ElapsedEventHandler(autoscaningButtons);// when timer is triggerred 'autoscaningButtons()' runs
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            indexHighlighted = 0; // index of element in List<Buttons> 


        }

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void autoscaningButtons(object source, ElapsedEventArgs e)
        {
            // increments index for next button
            if (indexHighlighted < currentButtons.Count - 1) { indexHighlighted++; }
            else { indexHighlighted = 0; }

            //currently highlighted button reverts to black background
            if (highlightedButton != null)
            {
                this.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    highlightedButton.Background = Brushes.Black;
                });
            }
            // change to next highlighted button
            highlightedButton = currentButtons[indexHighlighted];
            this.Dispatcher.Invoke(() => {
                highlightedButton.Background = Brushes.Red;
            });
        }

        //eventHandler for autscan button on XAML 
        private void Autoscan_Click(object sender, EventArgs e)
        {
            aTimer.Enabled = !aTimer.Enabled; //if timer is running disable it
            // enable key listeners
            if (aTimer.Enabled) //if timer is running
            {
                this.KeyDown += Key_down; // buttonPress EventHandler  -->  run "Key_down()" when any button is pressed on keyboard

            }
            else
            {
                this.KeyDown -= Key_down; // remove "Key_down" from KeyPressEvents

                //restore the highlighted key to original color
                if (highlightedButton != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        highlightedButton.Background = Brushes.Black;
                    });
                }
            }
            
        }

        // key press eventHandler
        private void Key_down(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            switch (k)
            {
                case Key.Q:
                    indexHighlighted -= 3; // go back 2 buttons (after Key_down autoscaningButtons is called, which adds 1 to index
                    if (indexHighlighted < 0)
                    {
                        indexHighlighted += currentButtons.Count; // loops the index if it goes negative
                    }
                    autoscaningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
                    aTimer.Stop(); //resets timer to give user consist 1000 ms to respond
                    aTimer.Start();
                    break;
                case Key.E:
                    highlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code
                    this.Dispatcher.Invoke(() => {
                        highlightedButton.Background = Brushes.DarkRed;
                    });
                    break;
            }

        }

        private void terminate_program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
