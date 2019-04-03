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


namespace Child_Talker
{
   
    class Autoscan
    {
        private static Autoscan instance;
        private MainWindow w;
        private TalkerView currentView;

        private Timer aTimer;
        private Button highlightedButton;
        private int indexHighlighted;

        private List<Button> currentButtons = new List<Button>(); //buttons being autoscanned

        private Autoscan()
        {
            aTimer = new Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(autoscanningButtons);// when timer is triggerred 'autoscanningButtons()' runs
            aTimer.AutoReset = true;
            aTimer.Enabled = false;           
        }

        public void startAutoscan(MainWindow _w)
        {
            w = _w;
            currentView = _w.DataContext as TalkerView;
            List<Button> thisButtons = new List<Button>();
            GetLogicalChildCollection(currentView, thisButtons);
            currentButtons = thisButtons;

            indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            currentView.KeyDown += Key_down;
        }

        public void updateAutoscan(DependencyObject parent)
        {
            stopAutoscan();

            List<Button> thisButtons = new List<Button>();
            GetLogicalChildCollection(parent, thisButtons);
            currentButtons = thisButtons;

            indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            currentView.KeyDown += Key_down;
        }

        public void stopAutoscan()
        {
            if (aTimer.Enabled)
            {
                currentView.KeyDown -= Key_down;
                aTimer.Enabled = false;
            }
        }

        public bool isScanning()
        {
            return (aTimer.Enabled);
        }
       
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {

            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)         //searching for type "T" which is usually Button
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                }
            }
        }
        public static Autoscan _instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Autoscan();
                }
                return instance;
            }
        }

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void autoscanningButtons(object source, EventArgs e)
        {
            // increments index for next button
            if (indexHighlighted < currentButtons.Count - 1) { indexHighlighted++; }
            else { indexHighlighted = 0; }

            //currently highlighted button reverts to black background
            if (highlightedButton != null)
            {
                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    highlightedButton.Background = Brushes.Black;
                });
                //highlightedButton.Click -= Autoscan_Click;
            }

            // change to next highlighted button
            highlightedButton = currentButtons[indexHighlighted];
            //highlightedButton.Click += Autoscan_Click;
            currentView.Dispatcher.Invoke(() => {
                highlightedButton.Background = Brushes.Red;
            });

           
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
                    autoscanningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
                    aTimer.Stop(); //resets timer to give user consist 1000 ms to respond
                    aTimer.Start();
                    break;
                case Key.E:
                    highlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code
                    currentView.Dispatcher.Invoke(() => {
                        highlightedButton.Background = Brushes.DarkRed;
                    });
                    break;
            }

        }
        
    }
 }