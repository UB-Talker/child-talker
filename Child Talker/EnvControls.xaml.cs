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

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for EnvControls.xaml
    /// </summary>
    public partial class EnvControls : Window
    {
        private ConsoleControls cc = new ConsoleControls();
        private List<Button> listButtons = new List<Button>();
        private Timer aTimer;
        private Button highlightedButton;
        private int indexHighlighted;

        public EnvControls()
        {
            InitializeComponent();
            GetLogicalChildCollection(this, listButtons);
            runTimer();
        }
    
        //  creates a list from all buttons on the page
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

        private void relayControl(object sender, RoutedEventArgs e)
        {
            string _tag = (((Button)sender).Tag).ToString();
            int _t = int.Parse(_tag);
            Debug.Print(_t.ToString());
            cc.relayControl(_t);
        }

        private void powerControl(object sender, RoutedEventArgs e)
        {
            cc.remoteControlSend("STV", "KEY_POWER");
        }

        //using System.Timers
        public void runTimer()
        {
            aTimer = new Timer(1000);

            aTimer.Elapsed += new ElapsedEventHandler(autoscaningButtons);
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            indexHighlighted = 0;


        }

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void autoscaningButtons(object source, ElapsedEventArgs e)
        {
            // increments index for next button
            if (indexHighlighted < listButtons.Count - 1) { indexHighlighted++; }
            else { indexHighlighted = 0; }

            //currently highlighted button reverts to black background
            if (highlightedButton != null)
            {
                this.Dispatcher.Invoke(() => {
                    highlightedButton.Background = Brushes.Black;
                });
            }
            // change to next highlighted button
            highlightedButton = listButtons[indexHighlighted];
            this.Dispatcher.Invoke(() => {
                highlightedButton.Background = Brushes.Red;
            });
        }

        private void Autoscan_Click(object sender, EventArgs e)
        {
            aTimer.Enabled = !aTimer.Enabled;
            // enable key listeners
            if (aTimer.Enabled)
            {
                layoutGrid.KeyDown += Key_down;
            }
            else
            {
                layoutGrid.KeyDown -= Key_down;
            }
            //restore the highlighted key to original color
            if (highlightedButton != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    highlightedButton.Background = Brushes.Black;
                });
            }
        }

        private void Key_down(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            if (k == Key.Q)
            {
                indexHighlighted -= 2;
                if (indexHighlighted < 0)
                {
                    aTimer.Stop();
                    indexHighlighted += listButtons.Count;
                    aTimer.Start();
                }
            }

            if (k == Key.E)
            {
                highlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
