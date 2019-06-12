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
using System.Windows.Forms;
using Child_Talker.TalkerViews;
using Control = System.Windows.Controls.Control;
using Panel = System.Windows.Controls.Panel;
using UserControl = System.Windows.Controls.UserControl;

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

            public UserControl currentView;
            public Panel parentPanel;

            // sets element to highlighted color
            public void HighlightElement()
            {//ln226
                currentView?.Dispatcher.Invoke( () =>
                {
                    switch (highlightedObject)
                    {
                        case Control control:
                            switch (control)
                            {
                                case TlkrBTN btn:
                                    originalBackground = control.Background;
                                    control.Background = highlightBackground;
                                    originalForeground = btn.TkrForeground;
                                    btn.TkrForeground = highlightForeground;
                                    break;
                                case Item item:
                                    control.Background = Brushes.Transparent;
                                    originalBackground = item.Background;
                                    item.Background = highlightBackground;
                                    originalForeground = item.TkrForeground;
                                    item.TkrForeground = highlightForeground;
                                    break;
                                default:
                                    originalBackground = control.Background;
                                    control.Background = highlightBackground;
                                    break;
                            }
                            break;

                        case Panel panel:
                            originalBackground = panel.Background;
                            panel.Background = highlightBackground;
                            break;
                    }
                });
            }
            // restores element to original color
            public void RestoreOriginalColor()
            {
                currentView?.Dispatcher.Invoke(() =>
                {
                    switch (highlightedObject)
                    {
                        // this is needed to change anything in xaml 
                        case Control control:
                            control.Background = originalBackground;
                            switch (control)
                            {
                                case TlkrBTN btn:
                                    btn.TkrForeground = originalForeground;
                                    break;
                                case Item item:
                                    control.Background = Brushes.Transparent;
                                    item.TkrForeground = originalForeground;
                                    item.Background = originalBackground;
                                    break;
                            }
                            break;

                        case Panel panel:
                            panel.Background = originalBackground;
                            break;
                    }
                });

            }
        }
        private readonly HighlightedElementInfo hei = new HighlightedElementInfo();
        private Panel returnPoint;

        public bool FlagPaused {
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

        public int FlagDirection
        {
            set => direction = value < 0 ? -1 : 1; 
            get => direction; 
        }
        public double FlagSpeed {
            set => autoscanTimer.Interval = value; 
            get => autoscanTimer.Interval; 
        }
        public bool FlagIsReturnPoint
        {
            set {
                isReturnPoint = value;
                if (value) { returnPoint = hei.parentPanel; }
            }
            get => isReturnPoint; 
        }
        public static bool flagAutoscanActive = false;

        // flag contents
        private bool isReturnPoint;
        private int direction = 1;
        private readonly Timer autoscanTimer;

        private enum ControlKeys
        {
            GoBack = Key.Up,
            Reverse = Key.Left,
            Select = Key.Right
        }
        /// <summary>
        /// sets the parent panel equal to null
        /// meaning that the next time GoBack is pressed will act as if at top most level of page
        /// </summary>
        public void ClearParentPanel()
        {
            hei.parentPanel = null;
        }

        
        //delegates are used to define parameter and return value for event listener

        public delegate void UIEventHandler(HighlightedElementInfo hei);

        //these are event handlers so that other classes can add events when any of these keys are pressed while autoscan is running
        public event UIEventHandler SelectPress;
        public event UIEventHandler SelectHold;
        public event UIEventHandler ReversePress;
        public event UIEventHandler ReverseHold;
        public event UIEventHandler GoBackPress;
        public event UIEventHandler GoBackHold;

        //these tell the system to ignore an actions default behavior
        public bool SelectDefaultEnabled = true;
        public bool ReverseDefaultEnabled = true;
        public bool GoBackDefaultEnabled = true;

        private static Autoscan _instance;
        private Window currentWindow;
        public UserControl CurrentView
        {   
            get => hei.currentView;  
            set => hei.currentView = value; 
        }

        private List<DependencyObject> currentObjectList = new List<DependencyObject>(); //buttons being autoscanned

        private Autoscan()
        {
            autoscanTimer = new Timer();
            FlagSpeed = 1000; // this sets the autoscanTimer.Interval

            autoscanTimer.Elapsed += AutoscanningButtons;// when timer is triggered 'AutoscanningButtons()' runs
            autoscanTimer.AutoReset = true;
            FlagPaused = true;
            flagAutoscanActive = false;
        }

        public Panel GetPanel()
        {
            return hei.parentPanel;
        }

        /// <summary>
        /// Initializes Autoscan with a user defined List of DependencyObjects to scan through, and the option of providing Panel that will be scanned next after GoBack is pressed (defaults to null if ignored)
        /// </summary>
        /// <param name="newObjectList">The list of DependencyObjects to scan</param>
        /// <param name="parentPanel"> Designated location what gets scanned next after GoBack is triggered (unless modifed elsewhere)</param>
        /// <param name="newWindow"> Will update active window if not null</param>
        public void StartAutoscan(List<DependencyObject> newObjectList, Panel parentPanel = null, Window newWindow=null) 
        {
            if(!flagAutoscanActive) return;
            UpdateActiveWindow(newWindow);

            autoscanTimer.Stop();
            flagAutoscanActive = true;
            if (hei.highlightedObject != null) { hei.RestoreOriginalColor(); } 
            hei.parentPanel = parentPanel;


            CurrentView = currentWindow.DataContext as UserControl;
            currentObjectList = newObjectList; //sets currentObjectList at parent objects to be scanned

            hei.indexHighlighted = 0;
            hei.highlightedObject = currentObjectList[hei.indexHighlighted];       //resets button so first button on new screens isn't skipped
            hei.HighlightElement();            

            flagAutoscanActive = true;
            FlagDirection = 1;
            autoscanTimer.Start();
        }


        /*
         * this is more automated Autoscan function 
         * provide it with a panel and tell it what type to look for (Buttons)
         * and it will isolate all immediate Button children from the parent
         * in this scenario if the parent has any Panel children they cannot be scanned with this method
         */
        public void StartAutoscan<T>(Panel parent, Window newWindow = null) where T : DependencyObject // allow you to specify a type within a panel or just collect all direct children elements
        {
            if(!flagAutoscanActive) return;
            if (hei.highlightedObject != null) { hei.RestoreOriginalColor(); } // just in case color was not reset elsewhere

            UpdateActiveWindow(newWindow);

            List<DependencyObject> tempObjectList = GetLogicalChildCollection<T>(parent, new List<DependencyObject>());
            if (tempObjectList.Count == 0) return;

            autoscanTimer.Stop();
            hei.parentPanel = parent;
            currentObjectList = tempObjectList;

            //special case code, panels that need a unique scanning process
            //kind of brute forcing, there's a better way to do this
            if (parent != null)
            {
                if (!FlagIsReturnPoint)
                {
                    FlagIsReturnPoint = TlkrPanel.isReturnPoint(parent);
                }

                FlagDirection = TlkrPanel.scanReverse(parent) ? -1 : 1;
            }
            else
            {
                FlagDirection = 1;
            }

            hei.indexHighlighted = 0;
            hei.highlightedObject = currentObjectList[hei.indexHighlighted]; //resets button so first button on new screens isn't skipped
            hei.HighlightElement();
            autoscanTimer.Start();

        }

        public void StopAutoscan()
        {
            if(!flagAutoscanActive) return;

            if (!autoscanTimer.Enabled) return;

            autoscanTimer.Stop();
            hei.RestoreOriginalColor();
            hei.parentPanel = null;
            currentWindow.KeyDown -= KeyDown;
            currentWindow.KeyUp -= KeyUp;
        }

        public bool IsScanning()
        {
            return (!FlagPaused);
        }
       
        public static bool IsActive()
        {
            return flagAutoscanActive;
        }
       
        // searches through parent for all children of type T searches recursively only when designated (i.e. if a border element is found)
        private static List<DependencyObject> GetLogicalChildCollection<T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            if(!flagAutoscanActive) return null;
            if (parent == null) { return null; }
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (var child in children)
            {
                if ( !(child is DependencyObject depChild) ) continue;

                if (depChild is T)         //searching for type "T" which is usually Button
                {
                    if (depChild is Panel || depChild is Control )
                    {
                        logicalCollection.Add(depChild);
                    }
                }
                else if (child is Decorator){
                    GetLogicalChildCollection<T>(depChild, logicalCollection); //If still in dependencyObject, go into depChild's children
                }
            }
            return logicalCollection;
        }

        public void AddToCollection(DependencyObject newDepObj) 
        {
            if(!flagAutoscanActive) return ;
            if (newDepObj is Panel || newDepObj is Button)
                currentObjectList.Add(newDepObj);
        }

        /// <summary>
        /// searches through parent for all children of type T searches recursively only when designated (i.e. if a border element is found)
        /// This is being used to as a last ditch scan to find any existing items of type T anywhere on the window 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="logicalCollection"></param>
        /// <returns></returns>
        public static List<DependencyObject> GenerateObjectList <T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            if(!flagAutoscanActive) return null;
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (var child in children)
            {
                if (!(child is DependencyObject depChild)) continue; // 'continue' stops this iteration and moves onto the next one

                if (depChild is T)         //searching for type "T" which is usually Button
                {
                    logicalCollection.Add(depChild);
                }
                GenerateObjectList<T>(depChild, logicalCollection); //If still in dependencyObject, go into depChild's children
            }
            return logicalCollection;
        }
        //autoscan is a singleton class, calling it requires instance instead of the constructor call
        //maintains only one _instance at all times
        public static Autoscan Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Autoscan();
                }
                _instance.FlagIsReturnPoint = false; // when a new page gets instance this must be made false
                return _instance;
            }
        }

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void AutoscanningButtons(object source, EventArgs e)
        {
            if(!flagAutoscanActive) return ;
            if (FlagPaused) return; //autoscanning doesn't happen while paused

            if (hei.highlightedObject != null)
            {
                hei.indexHighlighted += FlagDirection;
                if (currentObjectList.Count == 1) {
                    hei.indexHighlighted = 0;
                }
                else if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
                {
                    hei.indexHighlighted -= currentObjectList.Count * FlagDirection; // loops the index if it goes negative
                }
                //currently highlighted button reverts to original background
                hei.RestoreOriginalColor();
            }

            // change to next highlighted button
            hei.highlightedObject = currentObjectList[hei.indexHighlighted];

            if (hei.highlightedObject != null)
            {
                hei.HighlightElement();
                switch (hei.highlightedObject)
                {
                    case Button highlightedButton:
                        CurrentView.Dispatcher.Invoke(() =>
                        {
                            highlightedButton.BringIntoView();
                        });
                        break;
                    case Item highlightedItem:
                        CurrentView.Dispatcher.Invoke(() =>
                        {
                            highlightedItem.BringIntoView();
                        });
                        break;
                    case Panel highlightedPanel:
                        CurrentView.Dispatcher.Invoke(() =>
                        {
                            highlightedPanel.BringIntoView();
                        });
                        break;

                }
            }
        }
        
        // when a second window is used the button listeners must be moved to the new window
        public void UpdateActiveWindow(Window window)
        {
            if(!flagAutoscanActive) return ;
            if (window == null) return;

            hei.parentPanel = null;

            if (hei.highlightedObject != null)
            {
                // just in case color wasn't reset elsewhere
                hei.RestoreOriginalColor();
            }

            if (currentWindow != null)
            {
                currentWindow.KeyUp -= KeyUp;
                currentWindow.KeyDown -= KeyDown;
                hei.parentPanel = null;
            }

            window.KeyUp += KeyUp;
            window.KeyDown += KeyDown;
            currentWindow = window;
        }

        /*
         * the keyDown event occurs repeatedly while key is pressed
         * booleans are used to force it to only happen once
         *  
         * (QUESTION should reverse move faster in the opposite direction)
         */
        private bool reverseIsPressed = false; 
        private bool selectIsPressed = false;
        private bool goBackIsPressed = false;
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(!flagAutoscanActive) return ;
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
        /// <summary>
        /// will Skip Default behavior once immediately after Invocation is called
        /// </summary>
        public bool IgnoreReversePressOnce = false;
        public bool IgnoreSelectPressOnce = false;
        public bool IgnoreGoBackPressOnce = false;
        private void KeyUp(object sender, KeyEventArgs e)
        {
            if(!flagAutoscanActive) return ;
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse :
                    ReversePress?.Invoke(hei);
                    if(!IgnoreReversePressOnce)ReversePressDefault();
                    IgnoreReversePressOnce = false;
                    reverseIsPressed = false;
                    break;
                case (Key)ControlKeys.Select:
                    SelectPress?.Invoke(hei);
                    if(!IgnoreSelectPressOnce)SelectPressDefault();
                    IgnoreSelectPressOnce = false;
                    selectIsPressed = false;
                   break;
                case (Key)ControlKeys.GoBack:
                    GoBackPress?.Invoke(hei);
                    //this if statement is used so that the default can be ignored for only one press if set by user
                    if(!IgnoreGoBackPressOnce)GoBackPressDefault();
                    IgnoreGoBackPressOnce = false; 
                    goBackIsPressed = false;
                    break;
                default:
                    return;
            }
        }

        public void SelectPressDefault()
        {
            if (!SelectDefaultEnabled) return;
            switch (hei.highlightedObject)
            {
                case Panel panel:
                    hei.RestoreOriginalColor();
                    StartAutoscan<DependencyObject>(panel); //pass in panel that was clicked 
                    hei.parentPanel = panel;
                    break;
                case Button button:
                    if (button is TlkrBTN btn && btn.PauseOnSelect)
                    {
                        FlagPaused = true;
                    }
                    else if (FlagIsReturnPoint)
                    {
                        hei.RestoreOriginalColor();
                        StartAutoscan<DependencyObject>(returnPoint); //pass in panel that was clicked 
                        autoscanTimer.Start();
                    }
                    else
                    {
                        hei.RestoreOriginalColor();
                        autoscanTimer.Start();
                    }
                    button?.RaiseEvent(
                        new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase
                            .ClickEvent)); // how you simulate a button click in code
                    break;
                case Item item:
                    hei.RestoreOriginalColor();
                    item?.CtTile.PerformAction();
                    break;
            }

        }
        
        /// <summary>
        /// This is added to the invocation Record of the SecondaryWindow.Close() upon the window's instantiation
        /// ** see SecondaryWindow Constructor
        /// **** DO NOT MANUALLY CALL -- manual call will have no effect
        /// </summary>
        public void GoBackCloseSecondaryWindow(object sender, EventArgs e=null)
        {
            if(!flagAutoscanActive) return ;
            if (!(sender is SecondaryWindow windowToClose)) return;

            StopAutoscan(); //will be restarted by start autoscan
            UpdateActiveWindow(windowToClose.parentWindow);
            GoBackStartScan();
        }
        
        private void GoBackPressPaused()
        {
                FlagPaused = false;
                if (FlagIsReturnPoint)
                {
                    StartAutoscan<DependencyObject>(returnPoint); //pass in panel that was clicked 
                }
        }

        private void GoBackIsReturnPoint()
        {
            if (TlkrPanel.isReturnPoint(hei.parentPanel)) //if the current panel being scanned is a returnPoint then reset return Point
            {
                FlagIsReturnPoint = false;
                try
                {
                    StartAutoscan((currentWindow.DataContext as TalkerView).getParents());
                }
                catch
                {
                    // if it is 
                    hei.parentPanel = null;  
                    GoBackPressDefault();
                }
            }
            else
            {
                StartAutoscan<DependencyObject>(returnPoint);
            }
        }

        private void GoBackStartScan()
        {
            try
            {
                StartAutoscan((currentWindow.DataContext as TalkerView).getParents());
            }
            catch
            {
                // if get parents is not defined create list from all top level panels
                StartAutoscan(GetLogicalChildCollection<Panel>(currentWindow, new List<DependencyObject>()));
            }

        }

        private void GoBackUpdateMainWindow()
        {
                    if (!((MainWindow)currentWindow).backIsEmpty())
                    {
                        ((MainWindow)currentWindow).back();
                    }
        }
        public void GoBackPressDefault()
        {
            if (!GoBackDefaultEnabled) return;
            hei.RestoreOriginalColor();
            if (FlagPaused)
            {
                GoBackPressPaused();
            }
            else 
            if (hei.parentPanel != null)
            {
                if (FlagIsReturnPoint)
                {
                    GoBackIsReturnPoint();
                }
                else
                {
                    GoBackStartScan();
                }
            }
            else 
            if (hei.parentPanel == null)
            {
                if (currentWindow is SecondaryWindow)
                {
                    currentWindow.Close();
                    // For more info See 
                    // GoBackCloseSecondaryWindow
                }
                else
                {
                    GoBackUpdateMainWindow();
                    GoBackStartScan();
                }
            }
            else //impossible to reach but just in case
            {
                    GoBackStartScan();
            }
        }

        public void ReversePressDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            FlagDirection = -FlagDirection;
            autoscanTimer.Start();
        }

        public void ReverseHoldDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            //hei.indexHighlighted -= 1 * FLAG_direction; // go back 2 buttons (after KeyUp autoscanningButtons is called, which adds 1 to index           
            if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
            {
                hei.indexHighlighted += currentObjectList.Count * FlagDirection; // loops the index if it goes negative
            }

            FlagDirection = -FlagDirection;
            autoscanTimer.Start();
            AutoscanningButtons(null, null); //manually calls event handler so the q key press is executed immediately
        }
    }
 }