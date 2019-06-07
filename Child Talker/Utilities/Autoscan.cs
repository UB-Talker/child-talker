using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Timer = System.Timers.Timer;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Button = System.Windows.Controls.Button;
using System;
using Child_Talker.TalkerViews;

namespace Child_Talker.Utilities
{
    public class Autoscan
    {
        public class HighlightedElementInfo //this was added to organize the data a more
        {
            public DependencyObject highlightedObject;
            private Brush originalBackground;
            private Brush originalForeground;
            private readonly Brush highlightBackground = Brushes.Yellow;
            private readonly Brush highlightForeground = Brushes.Purple;
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

                        originalBackground = ((Control)highlightedObject).Background;
                        ((Control)highlightedObject).Background = highlightBackground;
                        if (highlightedObject is TlkrBTN)
                        {
                            originalForeground = ((TlkrBTN)highlightedObject).TkrForeground;
                            ((TlkrBTN)highlightedObject).TkrForeground = highlightForeground;
                        }
                        else if (highlightedObject is Item)
                        {
                            ((Control)highlightedObject).Background = Brushes.Transparent;
                            originalBackground = ((Item)highlightedObject).Background;
                            ((Item)highlightedObject).Background = highlightBackground;
                            originalForeground = ((Item)highlightedObject).TkrForeground;
                            ((Item)highlightedObject).TkrForeground = highlightForeground;
                        }                     }
                    else if (highlightedObject is Panel)
                    {
                        originalBackground = ((Panel)highlightedObject).Background;
                        ((Panel)highlightedObject).Background = highlightBackground;
                    }
                });
            }
            // restores element to original color
            public void restoreOriginalColor()
            {
                currentView.Dispatcher.Invoke(() => { // this is needed to change anything in xaml 
                    if (highlightedObject is Control)
                    {
                        ((Control)highlightedObject).Background = originalBackground;
                        if (highlightedObject is TlkrBTN)
                        {
                            ((TlkrBTN)highlightedObject).TkrForeground = originalForeground;
                        }
                        else if (highlightedObject is Item)
                        {
                            ((Control)highlightedObject).Background = Brushes.Transparent;
                            ((Item)highlightedObject).TkrForeground = originalForeground;
                            ((Item)highlightedObject).Background = originalBackground;
                        }
                    }
                    if (highlightedObject is Panel)
                    {
                        ((Panel)highlightedObject).Background = originalBackground;
                    }
                });

            }
        }
        private HighlightedElementInfo hei = new HighlightedElementInfo();
        private Panel returnPoint;

        public bool FLAG_paused {
            set
            {
                if (!value)
                {
                    if (!autoscanTimer.Enabled) { autoscanTimer.Start(); }
                }
                else
                {
                    if (autoscanTimer.Enabled) { autoscanTimer.Stop(); }
                }
            }
            get => !autoscanTimer.Enabled; 
        }
        private int direction = 1;
        public int FLAG_direction
        {
            set => direction = value < 0 ? -1 : 1; 
            get => direction; 
        }
        public double FLAG_Speed {
            set => autoscanTimer.Interval = value; 
            get => autoscanTimer.Interval; 
        }
        private bool isReturnPoint;
        public bool FLAG_isReturnPoint
        {
            set {
                isReturnPoint = value;
                if (value) { returnPoint = hei.parentPanel; }
            }
            get => isReturnPoint; 
        }
        private static bool FLAG_autoscanActive = false;

        private enum ControlKeys
        {
            GoBack = Key.Up,
            Reverse = Key.Left,
            Select = Key.Right
        }

        private Timer autoscanTimer;
        
        //delegates are used to define parameter and return value for event listener

        public delegate void UI_EventHandler(HighlightedElementInfo hei);

        //these are event handlers so that other classes can add events when any of these keys are pressed while autoscan is running
        public event UI_EventHandler SelectPress;
        public event UI_EventHandler SelectHold;
        public event UI_EventHandler ReversePress;
        public event UI_EventHandler ReverseHold;
        public event UI_EventHandler GoBackPress;
        public event UI_EventHandler GoBackHold;

        //these tell the system to ignore an actions default behavior
        public bool SelectDefaultEnabled = true;
        public bool ReverseDefaultEnabled = true;
        public bool GoBackDefaultEnabled = true;

        private static Autoscan _instance;
        private Window currentWindow;
        private TalkerView currentView
        {   
            get => hei.currentView;  
            set => hei.currentView = value; 
        }

        private List<DependencyObject> currentObjectList = new List<DependencyObject>(); //buttons being autoscanned

        private Autoscan()
        {
            autoscanTimer = new Timer();
            FLAG_Speed = 1000; // this sets the autoscanTimer.Interval

            autoscanTimer.Elapsed += autoscanningButtons;// when timer is triggerred 'autoscanningButtons()' runs
            autoscanTimer.AutoReset = true;
            FLAG_paused = true;
            FLAG_autoscanActive = false;
        }

        public Panel getPanel()
        {
            return hei.parentPanel;
        }

        /* 
         * this is a manual setup for autoscanning
         * by providing this with a list of dependency objects 
         * i.e a combination of buttons and panels
         * those items will be scanned
         * parentPanel is an optional parameter when set 'GoBackPress' will autoscan that element
         */
        public void startAutoscan(List<DependencyObject> newObjectList, Panel parentPanel = null) 
        {
            autoscanTimer.Stop();
            FLAG_autoscanActive = true;
            if (hei.highlightedObject != null) { hei.restoreOriginalColor(); } 
            hei.parentPanel = parentPanel;

            currentView = currentWindow.DataContext as TalkerView;
            currentObjectList = newObjectList; //sets currentObjectList at parent objects to be scanned

            hei.indexHighlighted = 0;
            hei.highlightedObject = currentObjectList[hei.indexHighlighted];       //resets button so first button on new screens isn't skipped
            hei.highlightElement();            

            FLAG_autoscanActive = true;
            FLAG_direction = 1;
            autoscanTimer.Start();
        }
        /*
         * this is more automated Autoscan function 
         * provide it with a panel and tell it what type to look for (Buttons)
         * and it will isolate all immediate Button children from the parent
         * in this scenario if the parent has any Panel children they cannot be scanned with this method
         */
        public void startAutoscan<T>(Panel parent) where T : DependencyObject // allow you to specify a type within a panel or just collect all direct children elements
        {
            if (hei.highlightedObject != null) { hei.restoreOriginalColor(); } // just in incase color was not reset elsewhere

            FLAG_autoscanActive = true;
            List<DependencyObject> tempObjectList = GetLogicalChildCollection<T>(parent, new List<DependencyObject>() );

            if (tempObjectList.Count !=0)
            {
                autoscanTimer.Stop();
                hei.parentPanel = parent;
                currentObjectList = tempObjectList;

                //special case code, panels that need a unique scanning process
                //kind of brute forcing, there's a better way to do this
                if(parent != null)
                {
                    if (!FLAG_isReturnPoint) { FLAG_isReturnPoint = TlkrPanel.isReturnPoint(parent); }
                    FLAG_direction = TlkrPanel.scanReverse(parent) ? -1 : 1 ;
                }
                else
                {
                    FLAG_direction = 1;
                }

                hei.indexHighlighted = 0;
                hei.highlightedObject = currentObjectList[hei.indexHighlighted];       //resets button so first button on new screens isn't skipped
                hei.highlightElement();            
                autoscanTimer.Start();
            }
        }
        
        public void stopAutoscan()
        {
            if (autoscanTimer.Enabled)
            {
                autoscanTimer.Stop();
                hei.restoreOriginalColor();
                hei.parentPanel = null;
                FLAG_autoscanActive = false;
            }
        }

        public bool isScanning()
        {
            return (!FLAG_paused);
        }
       
        public static bool isActive()
        {
            return FLAG_autoscanActive;
        }
       
        // searchs through parent for all children of type T searchs recursively only when designated (i.e. if a border element is found)
        private static List<DependencyObject> GetLogicalChildCollection<T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            if (parent == null) { return null; }
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (object child in children) 
            {
                DependencyObject depChild = child as DependencyObject;
                if (depChild != null)
                {
                    if (depChild is T)         //searching for type "T" which is usually Button
                    {
                        if (depChild is Panel || depChild is Control )
                        {
                            logicalCollection.Add(depChild);
                        }
                    }
                    else if (child is Decorator){
                       GetLogicalChildCollection<T>(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                    }
                }
            }
            return logicalCollection;
        }

        public void AddToCollection(DependencyObject newDepObj) 
        {
            if (newDepObj is Panel || newDepObj is Button)
                currentObjectList.Add(newDepObj);
        }

        // searchs through parent for all children of type T searchs recursively only when designated (i.e. if a border element is found)
        // This is being used to as a last ditch scan to find any existing items of type T anywhere on the window
        public static List<DependencyObject> generateObjectList <T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                DependencyObject depChild = child as DependencyObject;
                if (depChild != null)
                {
                    if (depChild is T)         //searching for type "T" which is usually Button
                    {
                        logicalCollection.Add(depChild);
                    }
                    generateObjectList<T>(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                }
            }
            return logicalCollection;
        }
        //autoscan is a singleton class, calling it requires instance instead of the constructor call
        //maintains only one _instance at all times
        public static Autoscan instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Autoscan();
                }
                _instance.FLAG_isReturnPoint = false; // TODO find a more prcise way to deal with this
                return _instance;
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
                    if (currentObjectList.Count == 1) {
                        hei.indexHighlighted = 0;
                    }
                    else if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
                    {
                        hei.indexHighlighted -= currentObjectList.Count * FLAG_direction; // loops the index if it goes negative
                    }
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
        
        // when a second window is used the button listeners must be moved to the new window
        public void updateActiveWindow(Window _window)
        {
            if (_window != null)
            {
                if (hei.highlightedObject != null)
                {
                    // just in case color wasn't reset elsewhere
                    hei.restoreOriginalColor();
                }

                if (currentWindow != null)
                {
                    currentWindow.KeyUp -= KeyUp;
                    currentWindow.KeyDown -= KeyDown;
                    hei.parentPanel = null;
                }
                currentWindow = _window;
                currentWindow.KeyUp += KeyUp;
                currentWindow.KeyDown += KeyDown;
            }
        }

        /*
         * what happens when holding down a key can only hold down one key at a time
         * the KeyDown event occurs once every time the q is pressed it will move the autoscan back once
         * until it is release autoscan will move in reverse
         *        (QUESTION should it move faster in the opposite direction)
         */
        private bool reverseIsPressed = false; 
        private bool selectIsPressed = false;
        private bool goBackIsPressed = false;
        private void KeyDown(object sender, KeyEventArgs e)
        {
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse when !reverseIsPressed:
                    reverseIsPressed = true;
                    ReverseHold?.Invoke(hei); // equivalent to saying if( ReverseHold != null ){ ReverseHold(); }  //this is a delegate Event for other objects to add methods
                    ReverseHoldDefault();
                    break;
                case (Key)ControlKeys.Select when !selectIsPressed:
                    selectIsPressed = true;
                    SelectHold?.Invoke(hei);
                    break;
                case (Key)ControlKeys.GoBack when !goBackIsPressed:
                    goBackIsPressed = true;
                    GoBackHold?.Invoke(hei);
                    break;
                default:
                    return;
            }
        }
        // key release eventHandler
        /* 
         * when any key is pressed this method is called 
         * only certain keys wil have an effect
         */ 
        private void KeyUp(object sender, KeyEventArgs e)
        {
            currentWindow.KeyUp -= KeyUp;  //added back at end so the event cannot be called twice before completion
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse :
                    reverseIsPressed = false;
                    ReversePress?.Invoke(hei);
                    ReversePressDefault();
                    break;
                case (Key)ControlKeys.Select:
                    selectIsPressed = false;
                    SelectPress?.Invoke(hei);
                    selectPressDefault();
                   break;
                case (Key)ControlKeys.GoBack:
                    goBackIsPressed = false;
                    GoBackPress?.Invoke(hei);
                    GoBackPressDefault();
                    break;
            }
            currentWindow.KeyUp += KeyUp;
        }


        private void selectPressDefault()
        {
            if (!SelectDefaultEnabled) return;
            if (hei.highlightedObject is Panel)
            {
                hei.restoreOriginalColor();
                Panel highlightedObject = (hei.highlightedObject as Panel);
                startAutoscan<DependencyObject>(highlightedObject); //pass in panel that was clicked 
                hei.parentPanel = highlightedObject;
            }
            else if ((hei.highlightedObject is Button) || (hei.highlightedObject is TlkrBTN))
            {
                Button highlightedObject = (hei.highlightedObject as Button);

                if (highlightedObject is TlkrBTN && ((TlkrBTN)highlightedObject).PauseOnSelect)
                {
                    FLAG_paused = true;
                }
                else if (FLAG_isReturnPoint)
                {
                    hei.restoreOriginalColor();
                    startAutoscan<DependencyObject>(returnPoint); //pass in panel that was clicked 
                    autoscanTimer.Start();
                }
                else
                {
                    hei.restoreOriginalColor();
                    autoscanTimer.Start();
                }

                highlightedObject.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
            }
            else if (hei.highlightedObject is Item)
            {
                hei.restoreOriginalColor();
                Item highlightedObject = (hei.highlightedObject as Item);
                highlightedObject.CtTile.PerformAction();
            }

        }

        private void GoBackPressDefault()
        {
            if (!GoBackDefaultEnabled) return;
            hei.restoreOriginalColor();
            if (FLAG_paused)
            {
                FLAG_paused = false;
                if (FLAG_isReturnPoint)
                {
                    startAutoscan<DependencyObject>(returnPoint); //pass in panel that was clicked 
                }
            }
            else if (hei.parentPanel != null)
            {
                if (FLAG_isReturnPoint)
                {
                    if (TlkrPanel.isReturnPoint(hei.parentPanel))
                    {
                        FLAG_isReturnPoint = false;
                        startAutoscan((currentWindow.DataContext as TalkerView).getParents());
                    }
                    else
                    {
                        startAutoscan<DependencyObject>(returnPoint);
                    }
                }
                else if (currentWindow is SecondaryWindow)
                {
                    currentWindow.Close();
                }
                else
                {
                    try
                    {
                        startAutoscan((currentWindow.DataContext as TalkerView).getParents());
                    }
                    catch
                    {
                        // if get parents is not defined create list from all top level panels
                        startAutoscan(GetLogicalChildCollection<Panel>(currentWindow, new List<DependencyObject>()));
                    }
                }
            }
            else if (hei.parentPanel == null)
            {
                MainWindow temp = currentWindow as MainWindow;
                if (temp != null && !temp.backIsEmpty())
                {
                    temp.back();
                }
            }
            else
            {
                startAutoscan(((TalkerView)currentWindow.DataContext).getParents());
            }
        }

        private void ReversePressDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            currentWindow.KeyDown += KeyDown;
            FLAG_direction = -FLAG_direction;
            reverseIsPressed = false;
            autoscanTimer.Start();
        }

        private void ReverseHoldDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            //hei.indexHighlighted -= 1 * FLAG_direction; // go back 2 buttons (after KeyUp autoscaningButtons is called, which adds 1 to index           
            if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
            {
                hei.indexHighlighted += currentObjectList.Count * FLAG_direction; // loops the index if it goes negative
            }

            FLAG_direction = -FLAG_direction;
            autoscanTimer.Start();
            autoscanningButtons(null, null); //manually calls event handler so the q key press is executed immedietly
        }
    }
 }