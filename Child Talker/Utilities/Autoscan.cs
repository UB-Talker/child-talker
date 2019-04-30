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
   
    public class Autoscan
    {
        private static Autoscan instance;
        private Window w;
        private TalkerView currentView;

        private Timer aTimer;
        private DependencyObject highlightedButton;
        private Panel parentPanel;
        private int indexHighlighted;
        private bool scanReversed = false;
        

        private List<DependencyObject> currentButtons = new List<DependencyObject>(); //buttons being autoscanned

        private Autoscan()
        {
            aTimer = new Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(autoscanningButtons);// when timer is triggerred 'autoscanningButtons()' runs
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            
        }

        
        public void startAutoscan<T>(Window _w) where T : DependencyObject
        {
        //called to iterate through view's "parent objects" which is a method getParents that the view should have
        //do not call on views that don't have getParents, only meant for views that need parent iteration for autoscan
            stopAutoscan();
            w = _w;
            currentView = _w.DataContext as TalkerView;
            currentButtons = currentView.getParents(); //sets currentButtons at parent objects to be scanned
            w.KeyDown += Key_down;

            indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            
            highlightedButton = null;       //resets button so first button on new screens isn't skipped
        }

        /* Finds objects(buttons usually) in Panel and starts autoscan on then
         * Only used when a specific panel needs to be scanned
         * Window param added bc there are certain instances in EnvControls that need to switch to a new window, and key_down stops working
        */
        public void partialAutoscan<T>(Panel parent, Window _w) where T : DependencyObject // T is a type. this function only works if T is Control type or Control Type dependent
        {
            stopAutoscan();
            w = _w;
            w.KeyDown += Key_down;
            List<DependencyObject> thisButtons = new List<DependencyObject>();
            parentPanel = parent;
            GetLogicalChildCollection<Button>(parent, thisButtons);
            currentButtons = thisButtons;

            //special case code, panels that need a unique scanning process
            //kind of brute forcing, there's a better way to do this
            if (String.Equals(parent.Name, "phraseStack")) //to reverse autoscan and start from bottom in history
            {
                indexHighlighted = currentButtons.Count - 1;
                scanReversed = true;
            }
            else if(String.Equals(parent.Name,"keyboardGrid"))
            {
               indexHighlighted = 0;
               currentButtons = (currentView as Child_Talker.TalkerViews.Keyboard).getRows();
            }
            else
            {
                indexHighlighted = 0; // index of element in List<Buttons>
            }
            
            aTimer.Enabled = true;
            highlightedButton = null;       //resets button so first button on new screens isn't skipped
        }


        public void stopAutoscan()
        {
            if (aTimer.Enabled)
            {
                w.KeyDown -= Key_down;
                aTimer.Enabled = false;
                scanReversed = false;
                parentPanel = null;
            }
        }

        public bool isScanning()
        {
            return (aTimer.Enabled);
        }
       
        //magic method, given a DependencyObject, will return a list of the children of type T in Parent
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

        //autoscan is a singleton class, calling it requires _instance instead of the constructor call
        //maintains only one instance at all times
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
                if (indexHighlighted < currentButtons.Count - 1 && !scanReversed) { indexHighlighted++; }
                else if (indexHighlighted >= currentButtons.Count && scanReversed) { indexHighlighted = 0; }
                else if (indexHighlighted > 0 && scanReversed) { indexHighlighted--; }
                else if (indexHighlighted <= 0 && scanReversed) { indexHighlighted = currentButtons.Count - 1; }
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
            
           
            if (parentPanel != null && highlightedButton is Control) {
                currentView.Dispatcher.Invoke(() =>
                {
                    (highlightedButton as Control).BringIntoView();
                });
               
            }
            
           
        }

    
        // key press eventHandler
        private void Key_down(object sender, KeyEventArgs e)
        {
            
            Key k = e.Key;
            switch (k)
            {
                case Key.Q:
                    if (scanReversed) { indexHighlighted += 3; }
                    else { indexHighlighted -= 3; } // go back 2 buttons (after Key_down autoscaningButtons is called, which adds 1 to index           
                    if (indexHighlighted < 0)
                    {
                        indexHighlighted += currentButtons.Count; // loops the index if it goes negative
                    }
                    autoscanningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
                    aTimer.Stop(); //resets timer to give user consist 1000 ms to respond
                    aTimer.Start();
                    break;
                case Key.E:
                    if (highlightedButton is Panel)
                    {
                        Panel oldHighlightedButton = (highlightedButton as Panel);

                        partialAutoscan<DependencyObject>(oldHighlightedButton, w);  //pass in panel that was clicked 
           
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldHighlightedButton.Background = Brushes.Black;
                        });
                    }
                    else
                    {
                        Control oldHighlightedButton = (highlightedButton as Control);
                        oldHighlightedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code

                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldHighlightedButton.Background = Brushes.Black;
                        });
                    }
                    break;
                case Key.S:
                    if (parentPanel != null && highlightedButton is Control)
                    {
                        Control oldHighlightedButton = (highlightedButton as Control);
                        startAutoscan<DependencyObject>(w);
                        currentView.Dispatcher.Invoke(() =>
                            {
                                oldHighlightedButton.Background = Brushes.Black;
                            });
                    }
                    else if(highlightedButton is Panel)
                    {
                        Panel oldHighlightedPanel = (highlightedButton as Panel);
                        startAutoscan<DependencyObject>(w);
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldHighlightedPanel.Background = Brushes.Black;
                        });
                    }
                    break;
            }

        }
        
    }
 }