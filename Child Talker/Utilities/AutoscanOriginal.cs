using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Child_Talker.TalkerButton;
using Timer = System.Timers.Timer;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Button = System.Windows.Controls.Button;
using Child_Talker.TalkerViews;
using Control = System.Windows.Controls.Control;
using Panel = System.Windows.Controls.Panel;
using UserControl = System.Windows.Controls.UserControl;

namespace Child_Talker.Utilities
{
    /// <summary>
    /// Key Functionality of Autoscan Class
    /// </summary>
    public partial class AutoscanOriginal
    {
        public class HighlightedElementInfo //this was added to organize the data a more
        {
            public DependencyObject highlightedObject;
            private Brush originalBackground;
            private static readonly Brush HighlightBackground = Brushes.Yellow;
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
                        case TalkerButton.Button btn:
                            btn.IsHighlighted = true;
                            break;

                        case Panel panel:
                            originalBackground = panel.Background;
                            panel.Background = HighlightBackground;
                            break;
                    }
                });
            }

            public void HighlightSelection()
            {
                currentView?.Dispatcher.Invoke(() => HighlightSelection(highlightedObject) );
            }
            public static void HighlightSelection(DependencyObject selectedObject)
            {//ln226
                Instance.CurrentView?.Dispatcher.Invoke(() =>
                {
                    switch (selectedObject)
                    {
                        case Control control:
                            switch (control)
                            {
                                case TalkerButton.Button btn:
                                    break;
                                default:
                                    break;
                            }

                            break;

                        case Panel panel:
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
                        case TalkerButton.Button btn:
                            btn.IsHighlighted = false;
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

        [Flags]
        public enum Flags
        {
            IsSecondWindow = 0,
            IsMainWindow    = 1,
            ReturnPoint     = 2,
            HasParentPanel  = 4,
            Paused          = 8,
        }
        private Flags flags;

        public bool ScanPause {
            set
            {
                if (!value)
                {
                    if ((flags & Flags.Paused)==Flags.Paused) {
                        autoscanTimer?.Start();
                        flags &= ~Flags.Paused; //set bit to 0
                    }
                }
                else
                {
                    if ((flags & Flags.Paused)!=Flags.Paused) {
                        autoscanTimer?.Stop();
                        flags |= Flags.Paused; //set bit to 1
                    }
                }
            }
            get => (flags & Flags.Paused)==Flags.Paused; 
        }

        private bool TimeToLeave = false;
        private int scanDirection = 1;
        public int ScanDirection
        {
            set => scanDirection = value < 0 ? -1 : 1; 
            get => scanDirection; 
        }
        public double FlagSpeed {
            set => autoscanTimer.Interval = value; 
            get => autoscanTimer.Interval; 
        }
        public bool IsReturnPoint
        {
            set
            {
                flags = value ? flags | Flags.ReturnPoint : flags & ~Flags.ReturnPoint;
                if (value)
                {
                    returnPoint = hei.parentPanel;
                }
            }
            get => (flags & Flags.ReturnPoint) == Flags.ReturnPoint;
        }

        private static bool _flagAutoscanActive = false;
        public static bool FlagAutoscanActive
        {
            set
            {
                if (value)
                {
                    Instance.ScanPause = false;
                    _flagAutoscanActive = true;
                }
                else
                {
                    Instance.ScanPause = true;
                    _flagAutoscanActive = false;
                }
            }
            get => _flagAutoscanActive;
        }

        private readonly Timer autoscanTimer;

        /// <summary>
        /// sets the parent panel equal to null
        /// meaning that the next time GoBack is pressed will act as if at top most level of page
        /// </summary>
        public void ClearParentPanel()
        {
            hei.parentPanel = null;
            
        }

        public void ClearReturnPoint()
        {
            returnPoint = null;
            IsReturnPoint = false;
        }
        
        private static AutoscanOriginal _instance;
        ///autoscan is a singleton class, calling it requires instance instead of the constructor call maintains only one _instance at all times
        public static AutoscanOriginal Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AutoscanOriginal();
                } 
                _instance.IsReturnPoint = false; // when a new page gets instance this must be made false
                return _instance;
            }
        }


        private Window currentWindow;
        public UserControl CurrentView
        {   
            get => hei.currentView;  
            set => hei.currentView = value; 
        }

        private List<DependencyObject> currentObjectList = new List<DependencyObject>(); //buttons being autoscanned

        private AutoscanOriginal()
        {
            autoscanTimer = new Timer();
            FlagSpeed = 1000; // this sets the autoscanTimer.Interval

            autoscanTimer.Elapsed += AutoscanningButtons;// when timer is triggered 'AutoscanningButtons()' runs
            autoscanTimer.AutoReset = true;
            autoscanTimer.Stop();
        }

        public Panel ParentPanel
        {
            set => hei.parentPanel = value;
            get => hei.parentPanel;
        }

        /// <summary>
        /// Initializes Autoscan with a user defined List of DependencyObjects to scan through, and the option of providing Panel that will be scanned next after GoBack is pressed (defaults to null if ignored)
        /// </summary>
        /// <param name="newObjectList">The list of DependencyObjects to scan</param>
        /// <param name="parentPanel"> Designated location what gets scanned next after GoBack is triggered (unless modifed elsewhere)</param>
        public void StartAutoscan(List<DependencyObject> newObjectList, Panel parentPanel=null) 
        {
            if(!FlagAutoscanActive) return;
            autoscanTimer.Stop();

            if (hei.highlightedObject != null) { hei.RestoreOriginalColor(); } 
            hei.parentPanel = parentPanel;
            CurrentView = currentWindow.DataContext as UserControl;
            TimeToLeave = true;
            currentObjectList = newObjectList; //sets currentObjectList at parent objects to be scanned

            hei.indexHighlighted = 0;
            hei.highlightedObject = currentObjectList[hei.indexHighlighted];       //resets button so first button on new screens isn't skipped
            hei.HighlightElement();            

            ScanDirection = 1;
            autoscanTimer.Start();
        }
        
        /// <summary>
        /// This is a more automated Autoscan function. 
        /// Provide it with a panel and tell it what type to look for (Usually Buttons)
        /// and it will isolate all immediate Button children from the parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="newWindow"></param>
        public void StartAutoscan<T>(Panel parent) where T : DependencyObject // allow you to specify a type within a panel or just collect all direct children elements
        {
            if(!FlagAutoscanActive) return;
            
            if (hei.highlightedObject != null) { hei.RestoreOriginalColor(); } // just in case color was not reset elsewhere

            List<DependencyObject> tempObjectList = GetLogicalChildCollection<T>(parent, new List<DependencyObject>());
            if (tempObjectList.Count == 0) return;

            autoscanTimer.Stop();
            hei.parentPanel = parent;
            TimeToLeave = false;
            currentObjectList = tempObjectList;

            if (parent == null)
            {
                ScanDirection = 1;
            }
            else
            {
                if (!IsReturnPoint)
                {
                    //IsReturnPoint = GetIsReturnPoint(parent);
                    returnPoint = parent;
                }
                //ScanDirection = GetScanReverse(parent) ? -1 : 1;
            }

            hei.indexHighlighted = 0;
            hei.highlightedObject = currentObjectList[hei.indexHighlighted]; //resets button so first button on new screens isn't skipped
            hei.HighlightElement();
            autoscanTimer.Start();

        }

        public void StopAutoscan()
        {
            if(!FlagAutoscanActive) return;
            if (!autoscanTimer.Enabled) return;
            hei.RestoreOriginalColor();

            autoscanTimer.Stop();
            hei.parentPanel = null;
            if (hei.highlightedObject is TalkerButton.Button btn)
            {
                btn.IsHighlighted = false;
            }
            currentWindow.KeyDown -= KeyDown;
            currentWindow.KeyUp -= KeyUp;
        }
       
        // searches through parent for all children of type T searches recursively only when designated (i.e. if a border element is found)
        public static List<DependencyObject> GetLogicalChildCollection<T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            if(!FlagAutoscanActive) return null;
            if (parent == null) { return null; }
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (var child in children)
            {
                if ( !(child is DependencyObject depChild) ) continue;

                if (depChild is T)         //searching for type "T" which is usually Button
                {
                    if (depChild is Panel || depChild is Control )
                    {
                        if (child is TalkerButton.Button btn && btn.DoNotScan) continue;
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
            if(!FlagAutoscanActive) return ;
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
            if(!FlagAutoscanActive) return null;
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

        // This method will get called by the timer until the timer stops or the program exits.
        // it changes the currently highlighted button for autoscanning
        public void AutoscanningButtons(object source, EventArgs e)
        {
            if(!FlagAutoscanActive) return ;
            if (ScanPause) return; //autoscanning doesn't happen while paused

            if (hei.highlightedObject != null)
            {
                hei.indexHighlighted += ScanDirection;
                if (currentObjectList.Count == 1) {
                    hei.indexHighlighted = 0;
                }
                else if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
                {
                    hei.indexHighlighted -= currentObjectList.Count * ScanDirection; // loops the index if it goes negative
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
            if(!FlagAutoscanActive) return ;
            if (window == null) return;

            hei.parentPanel = null;

            if (hei.highlightedObject != null)
            {
                hei.RestoreOriginalColor();
            }

            if (currentWindow != null)
            {
                currentWindow.KeyUp -= KeyUp;
                currentWindow.KeyDown -= KeyDown;
                hei.parentPanel = null;
            }

            flags = (window is MainWindow) ? flags | Flags.IsMainWindow : flags & ~Flags.IsMainWindow;
            window.KeyUp += KeyUp;
            window.KeyDown += KeyDown;
            currentWindow = window;
        }
    }

    /// <summary>
    /// EventHandler Functionality
    /// </summary>
    public partial class AutoscanOriginal
    {
        //delegates are used to define parameter and return value for event listener
        /// <summary>
        /// Event Handler type for all Autoscan Events passes a reference to the currently highlighted object
        /// </summary>
        /// <param name="hei"></param>
        public delegate void UIEventHandler(HighlightedElementInfo hei=null);

        /// occurs when a select button is pressed before default behavior occurs (to disable default behavior just once set ignoreSelectPressOnce = true)
        public event UIEventHandler SelectPress;
        /// occurs once when select is first held down (**may not exist depending on input method)
        public event UIEventHandler SelectHold;
        /// occurs when a reverse button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreReversePressOnce = true)
        public event UIEventHandler ReversePress;
        /// occurs once when Reverse is first held down (**may not exist depending on input method)
        public event UIEventHandler ReverseHold;
        /// occurs when a GoBack button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreGoBackPressOnce = true)
        public event UIEventHandler GoBackPress;
        /// occurs once when GoBack is first held down (**may not exist depending on input method)
        public event UIEventHandler GoBackHold;

        /// <summary>
        /// Disables default behavior until manually enabled using <see cref="resetSelectHandlers"/>
        /// <para> ** Use of <c>IgnoreSelectPressOnce</c> instead is highly recommended </para></summary>
        public bool SelectDefaultEnabled = true;                    
        /// <summary> Disables default behavior until manually enabled using <see cref="resetReverseHandlers"/>
        /// <para> ** Use of <c>IgnoreReversePressOnce</c> instead is highly recommended </para> </summary>
        public bool ReverseDefaultEnabled = true;                   
        /// <summary> Disables default behavior until manually enabled using <see cref="resetGoBackHandlers"/>
        /// <para> ** Use of <c>IgnoreGoBackPressOnce</c> instead is highly recommended </para></summary>
        public bool GoBackDefaultEnabled = true;
        
        /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreReversePressOnce = false;         /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreSelectPressOnce = false;          ///  If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreGoBackPressOnce = false;
        
        /// <summary>
        /// List of Keyboard Keys that are used for autoscan
        /// </summary>
        private enum ControlKeys
        {
            GoBack = Key.S,
            Reverse = Key.Q,
            Select = Key.E,
            GoBack2 = Key.Up,
            Reverse2 = Key.Left,
            Select2 = Key.Right
        }
        /// Clears EventHandlers (press & hold) and enables default behavior
        public void resetSelectHandlers()
        {
            SelectPress = null;
            SelectHold = null;
            SelectDefaultEnabled = true;
        }
        /// Clears EventHandlers (press & hold) and enables default behavior
        public void resetReverseHandlers()
        {
            ReversePress = null;
            ReverseHold = null;
            ReverseDefaultEnabled = true;
        }
        /// Clears EventHandlers (press & hold) and enables default behavior
        public void resetGoBackHandlers()
        {
            GoBackPress = null;
            GoBackHold = null;
            GoBackDefaultEnabled = true;
        }
        /// clears all Autoscan EventHandlers and restores default behavior
        public void resetHandlers()
        {
            if(_flagAutoscanActive) { autoscanTimer.Stop();}
            resetGoBackHandlers();
            resetReverseHandlers();
            resetSelectHandlers();
            if(_flagAutoscanActive) { autoscanTimer.Start();}
        }

        /// keeps hold event from happening repeatedly
        private bool reverseIsPressed = false; /// keeps hold event from happening repeatedly
        private bool selectIsPressed = false; /// keeps hold event from happening repeatedly
        private bool goBackIsPressed = false;

        /// triggers KeyHold behavior
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(!FlagAutoscanActive) return ;
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse when !reverseIsPressed:
                case (Key)ControlKeys.Reverse2 when !reverseIsPressed:
                    reverseIsPressed = true;
                    ReverseHold?.Invoke(hei); // equivalent to saying if( ReverseHold != null ){ ReverseHold(); }  //this is a delegate Event for other objects to add methods
                    ReverseHoldDefault();
                    break;
                case (Key)ControlKeys.Select when !selectIsPressed:
                case (Key)ControlKeys.Select2 when !selectIsPressed:
                    selectIsPressed = true;
                    SelectHold?.Invoke(hei);
                    break;
                case (Key)ControlKeys.GoBack when !goBackIsPressed:
                case (Key)ControlKeys.GoBack2 when !goBackIsPressed:
                    goBackIsPressed = true;
                    GoBackHold?.Invoke(hei);
                    break;
                default:
                    return;
            }
        }
        // triggers KeyPress behavior
        private void KeyUp(object sender, KeyEventArgs e)
        {
            if(!FlagAutoscanActive) return ;
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse :
                case (Key)ControlKeys.Reverse2 :
                    ReversePress?.Invoke(hei);
                    if(!IgnoreReversePressOnce)ReversePressDefault();
                    IgnoreReversePressOnce = false;
                    reverseIsPressed = false;
                    break;
                case (Key)ControlKeys.Select:
                case (Key)ControlKeys.Select2:
                    SelectPress?.Invoke(hei);
                    if(!IgnoreSelectPressOnce)SelectPressDefault();
                    IgnoreSelectPressOnce = false;
                    selectIsPressed = false;
                   break;
                case (Key)ControlKeys.GoBack:
                case (Key)ControlKeys.GoBack2:
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
                    if (button is TalkerButton.Button btn && btn.PauseOnSelect)
                    {
                        ScanPause = true;
                    }
                    else if (IsReturnPoint)
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
            }
        }
        
        /// <summary>
        /// This is added to the invocation Record of the SecondaryWindow.Close() upon the window's instantiation
        /// ** see SecondaryWindow Constructor
        /// **** DO NOT MANUALLY CALL -- manual call will have no effect
        /// </summary>
        public void GoBackCloseSecondaryWindow(object sender, EventArgs e=null)
        {
            if(!FlagAutoscanActive) return ;
            if (!(sender is SecondaryWindow windowToClose)) return;

            autoscanTimer.Stop();//will be restarted by start autoscan
            //UpdateActiveWindow(windowToClose.parentWindow);
            IsReturnPoint = false;
            ScanPause = false;
            returnPoint = null;
            resetHandlers();
            //GoBackStartScan();
        }

        private void GoBackPressPaused()
        {
            ScanPause = false;
            if (IsReturnPoint)
            {
                GoBackIsReturnPoint();
            }
        }

        private void GoBackIsReturnPoint()
        {
            //if (GetIsReturnPoint(hei.parentPanel)) //if the current panel being scanned is a returnPoint then reset return Point
            if(true)
            {
                IsReturnPoint = false;
                if(currentWindow.DataContext is TalkerView tv)
                {
                    StartAutoscan(tv.GetParents(), null);
                }
                else
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
            if (currentWindow.DataContext is TalkerView tv)
            {
                StartAutoscan(tv.GetParents(), null);
            }
            else
            { 
                // if get parents is not defined create list from all top level panels
                StartAutoscan(GetLogicalChildCollection<Panel>(currentWindow, new List<DependencyObject>()), null);
            }
        }

        private void GoBackUpdateMainWindow(MainWindow localCurrentWindow)
        {
            if (!localCurrentWindow.BackIsEmpty())
            {
                localCurrentWindow.Back();
            }
        }

        public void GoBackPressDefault()
        {
            if (!GoBackDefaultEnabled) return;
            hei.RestoreOriginalColor();

            if ((flags & Flags.Paused) == Flags.Paused)
            {
                GoBackPressPaused();
                return;
            }

            Flags gbFlags = flags & (Flags.ReturnPoint | Flags.HasParentPanel | Flags.IsMainWindow);
            switch (gbFlags)
            {
                case Flags.IsMainWindow | Flags.HasParentPanel | Flags.ReturnPoint: //111
                case Flags.IsSecondWindow | Flags.HasParentPanel | Flags.ReturnPoint: //110
                case Flags.IsMainWindow | Flags.ReturnPoint: //011 //no parentPanel
                case Flags.IsSecondWindow | Flags.ReturnPoint: //010
                    GoBackIsReturnPoint();
                    break;

                case Flags.IsMainWindow: //001
                    if (TimeToLeave)
                    {
                        GoBackUpdateMainWindow((MainWindow) currentWindow);
                    }
                    GoBackStartScan();
                    TimeToLeave = true;
                    break;
                case Flags.IsSecondWindow: //000
                        ((Window) currentWindow).Close();
                        break;


                case Flags.IsSecondWindow | Flags.HasParentPanel: //100 //no return point
                case Flags.IsMainWindow | Flags.HasParentPanel: //101 //
                    GoBackStartScan();
                    break;
            }
        }

        public void ReversePressDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            ScanDirection = -ScanDirection;
            autoscanTimer.Start();
        }

        public void ReverseHoldDefault()
        {
            if (!ReverseDefaultEnabled) return;
            autoscanTimer.Stop();
            //hei.indexHighlighted -= 1 * FLAG_direction; // go back 2 buttons (after KeyUp autoscanningButtons is called, which adds 1 to index           
            if (hei.indexHighlighted < 0 || hei.indexHighlighted >= currentObjectList.Count)
            {
                hei.indexHighlighted += currentObjectList.Count * ScanDirection; // loops the index if it goes negative
            }

            ScanDirection = -ScanDirection;
            autoscanTimer.Start();
            AutoscanningButtons(null, null); //manually calls event handler so the q key press is executed immediately
        }
    }
 }