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
        private Window w;
        private TalkerView currentView;

        private Timer aTimer;
        private DependencyObject highlightedButton;
        private int indexHighlighted;
        

        private List<DependencyObject> currentButtons = new List<DependencyObject>(); //buttons being autoscanned
        
        private bool scrollableView { get; set; }

        private Autoscan()
        {
            aTimer = new Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(autoscanningButtons);// when timer is triggerred 'autoscanningButtons()' runs
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            scrollableView = false;

            //System.Windows.Input.Keyboard.KeyDownEvent()
            //EventManager.RegisterClassHandler(typeof(Window), System.Windows.Input.Keyboard.KeyDownEvent, new KeyEventHandler(Key_down), true);

        }

        public void startAutoscan<T>(Window _w) where T : DependencyObject
        {
            w = _w;
            currentView = _w.DataContext as TalkerView;
            List<DependencyObject> thisButtons = new List<DependencyObject>();
            GetLogicalChildCollection<T>(currentView, thisButtons);
            currentButtons = thisButtons;
           
            indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            w.KeyDown += Key_down;
            highlightedButton = null;       //resets button so first button on new screens isn't skipped
          
        }

        public void updateAutoscan<T>(Panel parent) where T : DependencyObject // T is a type. this function only works if T is Control type or Control Type dependent
        {
            stopAutoscan();
           
            List<DependencyObject> thisButtons = new List<DependencyObject>();
            GetLogicalChildCollection<T>(parent, thisButtons);
            currentButtons = thisButtons;

            indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            w.KeyDown += Key_down;
            highlightedButton = null;       //resets button so first button on new screens isn't skipped
        }

        public void scanParents(List<DependencyObject> parents) 
        {
            if(aTimer.Enabled)
            {
                aTimer.Enabled = false;
            }
            //currentButtons = parents;

        }

        public void stopAutoscan()
        {
            if (aTimer.Enabled)
            {
                w.KeyDown -= Key_down;
                aTimer.Enabled = false;
            }
        }

        public bool isScanning()
        {
            return (aTimer.Enabled);
        }
       
        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)         //searching for type "T" which is usually Button
                    {
                        logicalCollection.Add(child as DependencyObject);
                    }
                    if (!(child is Control)){
                        GetLogicalChildCollection<T>(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                    }
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
           if (highlightedButton != null)
            {
                if (indexHighlighted < currentButtons.Count - 1) { indexHighlighted++; }
                else { indexHighlighted = 0; }

            //currently highlighted button reverts to black background
            
                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    if (highlightedButton is Control)
                    {
                        (highlightedButton as Control).Background = Brushes.Black;
                    }
                    if (highlightedButton is Panel)
                    {
                    (highlightedButton as Panel).Background = Brushes.Black;
                    }
                });
                
            }

            // change to next highlighted button
            highlightedButton = currentButtons[indexHighlighted];

            if (highlightedButton != null)
            {
                currentView.Dispatcher.Invoke(() =>
                {
                    if (highlightedButton is Control)
                    {
                        (highlightedButton as Control).Background = Brushes.Red;
                    }
                    if (highlightedButton is Panel)
                    {
                        (highlightedButton as Panel).Background = Brushes.Red;
                    }
                });
            }
            //if(scrollableView) { currentView.scrollDown(); }
           
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
                    if (highlightedButton is Control)
                    {
                        Control oldHighlightedButton = (highlightedButton as Control);
                        oldHighlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code

                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldHighlightedButton.Background = Brushes.Black;
                        });
                    }

                    break;
                
            }

        }
        
    }
 }