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
        private class HighlightedElementInfo //this was added to organize the data a more
        {
            public Panel parentPanel;
            public DependencyObject highlightedObject;
            public Brush originalColor;
            public Brush highlightColor = Brushes.Red;
            public int indexHighlighted;
        }

        private HighlightedElementInfo hei = new HighlightedElementInfo();

        private static Autoscan instance;
        private Window w;
        private TalkerView currentView;

        private Timer aTimer;
        private bool scanReversed = false;

        private List<DependencyObject> currentObjectList = new List<DependencyObject>(); //buttons being autoscanned

        private Autoscan()
        {
            aTimer = new Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(autoscanningButtons);// when timer is triggerred 'autoscanningButtons()' runs
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            
        }

        public Panel getPanel()
        {
            return hei.parentPanel;
        }
        
        public void startAutoscan<T>(Window _w) where T : DependencyObject
        {
        //called to iterate through view's "parent objects" which is a method getParents that the view should have
        //do not call on views that don't have getParents, only meant for views that need parent iteration for autoscan
            stopAutoscan();
            w = _w;
            currentView = _w.DataContext as TalkerView;
            currentObjectList = currentView.getParents(); //sets currentObjectList at parent objects to be scanned
            w.KeyDown += Key_down;

            hei.indexHighlighted = 0; // index of element in List<Buttons>
            aTimer.Enabled = true;
            
            hei.highlightedObject = null;       //resets button so first button on new screens isn't skipped
        }

        /* Finds objects(buttons usually) in Panel and starts autoscan on then
         * Only used when a specific panel needs to be scanned
         * Window param added bc there are certain instances in EnvControls that need to switch to a new window, and key_down stops working
        */
        public void partialAutoscan<T>(Panel parent) where T : DependencyObject // T is a type. this function only works if T is Control type or Control Type dependent
        {
            List<DependencyObject> tempObjectList = new List<DependencyObject>();

            GetLogicalChildCollection<T>(parent, tempObjectList);

            if (tempObjectList.Count !=0)
            {
                stopAutoscan();
                hei.parentPanel = parent;
                w.KeyDown += Key_down;
                currentObjectList = tempObjectList;

                //special case code, panels that need a unique scanning process
                //kind of brute forcing, there's a better way to do this
                if (String.Equals(parent.Name, "phraseStack")) //to reverse autoscan and start from bottom in history
                {
                    hei.indexHighlighted = currentObjectList.Count - 1;
                    scanReversed = true;
                }
                else if (String.Equals(parent.Name, "keyboardGrid"))
                {
                    hei.indexHighlighted = 0;
                    currentObjectList = (currentView as Child_Talker.TalkerViews.Keyboard).getRows();
                }
                else
                {
                    hei.indexHighlighted = 0; // index of element in List<Buttons>
                }

                aTimer.Enabled = true;
                hei.highlightedObject = null;       //resets button so first button on new screens isn't skipped
            }
        }


        public void stopAutoscan()
        {
            if (aTimer.Enabled)
            {
                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    if (hei.highlightedObject is Control)
                    {
                        (hei.highlightedObject as Control).Background = hei.originalColor;
                    }
                    if (hei.highlightedObject is Panel)
                    {
                    (hei.highlightedObject as Panel).Background = hei.originalColor;
                    }
                });

                w.KeyDown -= Key_down;
                aTimer.Enabled = false;
                scanReversed = false;
                hei.parentPanel = null;
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
                    else if (typeof(T) == typeof(Button))
                    {
                       if (child is Item)
                       {
                            logicalCollection.Add(child as DependencyObject);
                       } 
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
        
            if (hei.highlightedObject != null)
            {
                if (hei.indexHighlighted < currentObjectList.Count - 1 && !scanReversed) { hei.indexHighlighted++; }
                else if (hei.indexHighlighted >= currentObjectList.Count && scanReversed) { hei.indexHighlighted = 0; }
                else if (hei.indexHighlighted > 0 && scanReversed) { hei.indexHighlighted--; }
                else if (hei.indexHighlighted <= 0 && scanReversed) { hei.indexHighlighted = currentObjectList.Count - 1; }
                else { hei.indexHighlighted = 0; }

                //currently highlighted button reverts to black background

                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    if (hei.highlightedObject is Control)
                    {
                        (hei.highlightedObject as Control).Background = hei.originalColor;
                    }
                    if (hei.highlightedObject is Panel)
                    {
                    (hei.highlightedObject as Panel).Background = hei.originalColor;
                    }
                });
                
            }

            // change to next highlighted button
            hei.highlightedObject = currentObjectList[hei.indexHighlighted];

            if (hei.highlightedObject != null)
            {

                currentView.Dispatcher.Invoke(() =>
                {
                    if (hei.highlightedObject is Control)
                    {
                        
                        hei.originalColor = (hei.highlightedObject as Control).Background;
                        (hei.highlightedObject as Control).Background = hei.highlightColor;
                    }
                    if (hei.highlightedObject is Panel)
                    {
                        hei.originalColor = (hei.highlightedObject as Panel).Background;
                        (hei.highlightedObject as Panel).Background = hei.highlightColor;
                    }
                });
            }
            
           
            if (hei.parentPanel != null && hei.highlightedObject is Control) {
                currentView.Dispatcher.Invoke(() =>
                {
                    (hei.highlightedObject as Control).BringIntoView();
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
                    if (scanReversed) { hei.indexHighlighted += 3; }
                    else { hei.indexHighlighted -= 3; } // go back 2 buttons (after Key_down autoscaningButtons is called, which adds 1 to index           
                    if (hei.indexHighlighted < 0)
                    {
                        hei.indexHighlighted += currentObjectList.Count; // loops the index if it goes negative
                    }
                    autoscanningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
                    aTimer.Stop(); //resets timer to give user consist 1000 ms to respond
                    aTimer.Start();
                    break;
                case Key.E:
                    if (hei.highlightedObject is Panel)
                    {
                        Panel oldhighlightedObject = (hei.highlightedObject as Panel);

                        partialAutoscan<Button>(oldhighlightedObject);  //pass in panel that was clicked 
           
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldhighlightedObject.Background = hei.originalColor;
                        });
                    }
                    else if (hei.highlightedObject is Button)
                    {
                        Control oldhighlightedObject = (hei.highlightedObject as Control);
                        
                        oldhighlightedObject.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldhighlightedObject.Background = hei.originalColor;
                        });
                    }
                    else if (hei.highlightedObject is Item)
                    {
                        Item oldhighlightedObject = (hei.highlightedObject as Item);

                        oldhighlightedObject.CtTile.PerformAction();
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldhighlightedObject.Background = hei.originalColor;
                        });
                    }
                    break;
                case Key.S:
                    if (hei.parentPanel != null && hei.highlightedObject is Control)
                    {
                        Control oldhighlightedObject = (hei.highlightedObject as Control);
                        startAutoscan<DependencyObject>(w);
                        currentView.Dispatcher.Invoke(() =>
                            {
                                oldhighlightedObject.Background = hei.originalColor;
                            });
                    }
                    else if(hei.highlightedObject is Panel)
                    {
                        Panel oldHighlightedPanel = (hei.highlightedObject as Panel);
                        startAutoscan<DependencyObject>(w);
                        currentView.Dispatcher.Invoke(() =>
                        {
                            oldHighlightedPanel.Background = hei.originalColor;
                        });
                    }
                    break;
            }

        }
        
    }
 }