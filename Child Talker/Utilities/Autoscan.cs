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
    class IFlags
    {
         bool paused { set; get; }
        int direction { set; get; }

        uint speed { set; get; }
        bool isReturnPoint { set; get; }
        Autoscan.HighlightedElementInfo returnPoint { set; get; }
    }


    public class Autoscan 
    {
        internal class HighlightedElementInfo //this was added to organize the data a more
        {
            public DependencyObject highlightedObject;
            private Brush originalColor;
            private Brush highlightColor = Brushes.Red;
            public int indexHighlighted;

            public TalkerView currentView;
            public Panel parentPanel;

            // sets element to highlighted color
            public void highlightElement()
            {//ln226
                currentView.Dispatcher.Invoke(() =>
                {
                    if (highlightedObject is Control)
                    {
                        originalColor = (highlightedObject as Control).Background;
                        (highlightedObject as Control).Background = highlightColor;
                    }
                    if (highlightedObject is Panel)
                    {
                        originalColor = (highlightedObject as Panel).Background;
                        (highlightedObject as Panel).Background = highlightColor;
                    }
                });
            }
            // restores element to original color
            public void restoreOriginalColor()
            {
                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    if (highlightedObject is Control)
                    {
                        (highlightedObject as Control).Background = originalColor;
                    }
                    if (highlightedObject is Panel)
                    {
                        (highlightedObject as Panel).Background = originalColor;
                    }
                });

            }
        }
        private HighlightedElementInfo hei = new HighlightedElementInfo();
        private Panel returnPoint;

        private bool FLAG_paused {
            set
            {
                if (!value)
                {
                    if (!aTimer.Enabled) { aTimer.Start(); }
                }
                else
                {
                    if (aTimer.Enabled) { aTimer.Stop(); }
                }
            }
            get { return !aTimer.Enabled;  }
        }
        private int FLAG_direction = 1;
        private uint FLAG_speed = 1000;
        private bool isReturnPoint;
        private bool FLAG_isReturnPoint
        {
            set {
                isReturnPoint = value;
                if (value) { returnPoint = hei.parentPanel;
                }
            }
            get { return isReturnPoint; }
        }



        public Timer aTimer;
        
        private static Autoscan instance;
        private Window w;
        private TalkerView currentView
        {   
            get { return hei.currentView;  }
            set { hei.currentView = value; }
        }

        private List<DependencyObject> currentObjectList = new List<DependencyObject>(); //buttons being autoscanned

        private Autoscan()
        {
            aTimer = new Timer(FLAG_speed);

            aTimer.Elapsed += new ElapsedEventHandler(autoscanningButtons);// when timer is triggerred 'autoscanningButtons()' runs
            aTimer.AutoReset = true;
            FLAG_paused = true; 
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
            FLAG_direction = 1;
        }

        /* Finds objects(buttons usually) in Panel and starts autoscan on then
         * Only used when a specific panel needs to be scanned
         * Window param added bc there are certain instances in EnvControls that need to switch to a new window, and key_down stops working
        */
        public void partialAutoscan<T>(Panel parent) where T : DependencyObject // T is a type. this function only works if T is Control type or Control Type dependent
        {
            hei.indexHighlighted = 0;
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
                if( parent is TlkrGrid )
                {
                    if (!FLAG_isReturnPoint) FLAG_isReturnPoint = (parent as TlkrGrid).isReturnPoint;
                    FLAG_direction = (parent as TlkrGrid).scanReverse ? -1 : 1 ;
                }
                else if( parent is TlkrStackPanel )
                {
                    if (!FLAG_isReturnPoint) FLAG_isReturnPoint = (parent as TlkrStackPanel).isReturnPoint;
                    FLAG_direction = (parent as TlkrStackPanel).scanReverse ? -1 : 1 ;
                }
                else
                {
                    FLAG_direction = 1;
                }

                aTimer.Enabled = true;
                hei.highlightedObject = null;       //resets button so first button on new screens isn't skipped
            }
        }


        public void stopAutoscan()
        {
            if (aTimer.Enabled)
            {
                hei.restoreOriginalColor();

                w.KeyDown -= Key_down;
                aTimer.Enabled = false;
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
                        if (child is Panel || child is Control )
                        {
                            logicalCollection.Add(child as DependencyObject);
                        }
                    }
                    else if (typeof(T) == typeof(Button))
                    {
                       if (child is Item)
                       {
                            logicalCollection.Add(child as DependencyObject);
                       } 
                    }
                    if (child is Border){
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
                instance.FLAG_direction = 1;
                instance.FLAG_isReturnPoint = false;
                return instance;
            }
        }

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void autoscanningButtons(object source, EventArgs e)
        {
            if (!FLAG_paused)
            {
                if (hei.highlightedObject != null)
                {
                    hei.indexHighlighted += FLAG_direction;
                    if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
                    {
                        hei.indexHighlighted -= currentObjectList.Count * FLAG_direction;
                    }
                    if (currentObjectList.Count == 1) { hei.indexHighlighted = 0; }
                    //currently highlighted button reverts to original background
                    hei.restoreOriginalColor();
                }

                // change to next highlighted button
                hei.highlightedObject = currentObjectList[hei.indexHighlighted];

                if (hei.highlightedObject != null)
                {
                    hei.highlightElement();
                }


                if (hei.parentPanel != null && hei.highlightedObject is Control)
                {
                    currentView.Dispatcher.Invoke(() =>
                    {
                        (hei.highlightedObject as Control).BringIntoView();
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
                    hei.indexHighlighted -= 2*FLAG_direction; // go back 2 buttons (after Key_down autoscaningButtons is called, which adds 1 to index           
                    if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
                    {
                        hei.indexHighlighted += currentObjectList.Count*FLAG_direction; // loops the index if it goes negative
                    }
                    autoscanningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
                    aTimer.Stop(); //resets timer to give user consist 1000 ms to respond
                    aTimer.Start();
                    break;
                case Key.E:
                    if (hei.highlightedObject is Panel)
                    {
                        hei.restoreOriginalColor();
                        Panel oldhighlightedObject = (hei.highlightedObject as Panel);
                        partialAutoscan<DependencyObject>(oldhighlightedObject);  //pass in panel that was clicked 
                    }
                    else if ((hei.highlightedObject is Button) || (hei.highlightedObject is TlkrBTN))
                    {
                        Control oldhighlightedObject = (hei.highlightedObject as Control);
                        DependencyObject temp = hei.highlightedObject;
                        oldhighlightedObject.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // how you simulate a button click in code
                        if (oldhighlightedObject is TlkrBTN && (oldhighlightedObject as TlkrBTN).PauseOnSelect)
                        {
                            FLAG_paused = true;
                            hei.highlightedObject = temp;
                        }
                        else if (FLAG_isReturnPoint)
                        {
                            hei.restoreOriginalColor();
                            partialAutoscan<DependencyObject>(returnPoint);  //pass in panel that was clicked 
                        }
                    }
                    else if (hei.highlightedObject is Item)
                    {
                        hei.restoreOriginalColor();
                        Item oldhighlightedObject = (hei.highlightedObject as Item);
                        oldhighlightedObject.CtTile.PerformAction();
                    }
                   break;
                case Key.S:
                    if (FLAG_paused)
                    {
                        FLAG_paused = false;
                        if (FLAG_isReturnPoint)
                        {
                            hei.restoreOriginalColor();
                            partialAutoscan<DependencyObject>(returnPoint);  //pass in panel that was clicked 
                        }
                    }
                    else if (hei.parentPanel is TlkrGrid)
                    {
                        if ((hei.parentPanel as TlkrGrid).isReturnPoint && FLAG_isReturnPoint)
                        {
                            FLAG_isReturnPoint = false;
                        }
                        startAutoscan<DependencyObject>(w);
                        hei.restoreOriginalColor();
                    }
                    else if (hei.parentPanel is TlkrStackPanel){
                        if ((hei.parentPanel as TlkrStackPanel).isReturnPoint && FLAG_isReturnPoint)
                        {
                            FLAG_isReturnPoint = false;
                        }
                        startAutoscan<DependencyObject>(w);
                        hei.restoreOriginalColor();
                    }
                    else if (hei.parentPanel==null)
                    {
                       // go to previous page 
                       
                        // else
                        startAutoscan<DependencyObject>(w);
                        hei.restoreOriginalColor();
                    }
                    else
                    {
                        startAutoscan<DependencyObject>(w);
                        hei.restoreOriginalColor();
                    }

                    
                    break;
            }

        }
        
    }
 }