using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Child_Talker.Utilities
{ 
    /// <summary>
    /// this section is being used for Attached DependencyProperties
    /// </summary>
    public partial class Autoscan2 
    {
        /// <summary>
        /// tells autoscan which direction to scan the panels immediate children 
        /// <para>    true - scan reverse             </para>
        /// <para>   false - scan forward  (default)  </para>
        /// </summary>
        public static readonly DependencyProperty ScanReverseProperty = DependencyProperty.RegisterAttached(
            "ScanReverse",                              // Name of property on XAML
            typeof(bool),                               // type of property on XAML
            typeof(Autoscan2),                    // who inherits this property
            new UIPropertyMetadata(null));              // eventHandler if property changes

        /// <summary>
        /// when a child UserControl (button or item) is selected return here instead of immediate parent panel 
        /// (this is how the keyboard functions) - might not matter if first children are UserControls
        /// <para>   true - return scan to this element     </para>
        /// <para>   false - restart current panel after select    (default)</para>
        /// </summary>
        public static readonly DependencyProperty IsReturnPointProperty = DependencyProperty.RegisterAttached(
            "IsReturnPoint",                            // Name of property on XAML
            typeof(bool),                               // type of property on XAML
            typeof(Autoscan2),                    // who inherits this property
            new UIPropertyMetadata(null));              // eventHandler if property changes

        /* TODO
         */
        /// <summary>
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item              </para>
        /// <para>    false - autoscan continues as normal     (default) </para>
        /// </summary>
        public static readonly DependencyProperty DoNotScanProperty = DependencyProperty.RegisterAttached(
            "DoNotScan",                             // Name of property on XAML
            typeof(bool),                     // type of property on XAML
            typeof(Autoscan2),                    // who inherits this property
            new UIPropertyMetadata(false));              // eventHandler if property changes

        //the actual variable used to set and get the dependency property
        public static bool GetScanReverse(DependencyObject obj) => (bool)obj.GetValue(ScanReverseProperty);
        public static void SetScanReverse(DependencyObject obj, bool value) => obj.SetValue(ScanReverseProperty, value);

        public static bool GetIsReturnPoint(DependencyObject obj) => (bool)obj.GetValue(IsReturnPointProperty);
        public static void SetIsReturnPoint(DependencyObject obj, bool value) => obj.SetValue(ScanReverseProperty, value);

        public static bool GetDoNotScan(DependencyObject obj) => (bool)obj.GetValue(DoNotScanProperty);
        public static void SetDoNotScan(DependencyObject obj, bool value) => obj.SetValue(DoNotScanProperty, value);

        public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.RegisterAttached(
            "IsHighlight",                            // Name of property on XAML
            typeof(bool),                               // type of property on XAML
            typeof(Autoscan2),                    // who inherits this property
            new UIPropertyMetadata(false));              // eventHandler if property changes

        public static bool GetIsHighlight(DependencyObject obj) => (bool)obj.GetValue(IsHighlightProperty) ;
        public static void SetIsHighlight(DependencyObject obj, bool value) =>  obj.SetValue(IsHighlightProperty, value) ;

    }
    public partial class Autoscan2
    { 
         //delegates are used to define parameter and return value for event listener
        /// <summary>
        /// Event Handler type for all Autoscan Events passes a reference to the currently highlighted object
        /// </summary>
        /// <param name="currentObject"></param>
        public delegate void UIEventHandler(DependencyObject currentObject);

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
    }
    public partial class Autoscan2
    {
        private static Autoscan2 _instance;
        public static Autoscan2 Instance => _instance ?? (_instance = new Autoscan2());

        private const double scanTimerInterval = 1000;
        private readonly Timer scanTimer;

        public Stack<List<DependencyObject>> ReturnPointList { get; set; }
        private bool popReturnPointList = false;
        /// <summary>
        /// Clears all returnPoints for currentPage must occur every time the page is updated
        /// </summary>
        public void ClearReturnPointList() => ReturnPointList = new Stack<List<DependencyObject>>();
        private Stack<Window> windowHistoryPath;

       // private List<DependencyObject> Active;
        private List<DependencyObject> activeScanList;
        private DependencyObject currentScanObject;
        private int currentScanIndex = 0;

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

        public void PauseScan(bool toggle)
        {
            if( TimerMode == TimerModes.Off) return;
            if (toggle)
            {
                scanTimer.Stop();
                TimerMode = TimerModes.Paused;
            }
            else
            {
                scanTimer.Start();
                TimerMode = TimerModes.On;
            }

        }

        /// <summary>
        /// off/on - says Autoscan is either on or off
        /// Manual - Timer is Disabled Movement can still occur by pressing Reverse
        /// Paused - Timer and ReversePress are Disabled but SelectPress and GoBackPress Function normally
        /// </summary>
        public enum TimerModes { Off = 0, On = 1, Manual = 2, Paused = 3, }
        public TimerModes TimerMode { get; protected set; } = 0;

        public enum DirectionEnum { Forward = 1, Reverse = -1 };
        public DirectionEnum Direction { get; private set; } = DirectionEnum.Forward;

        private Autoscan2()
        {
            scanTimer = new Timer(scanTimerInterval) { AutoReset = true };
            scanTimer.Elapsed += ScanNextItem;
            scanTimer.Stop();
        }

        public void ToggleAutoscan()
        {
            if (TimerMode == TimerModes.Off)
            {
                TimerMode = TimerModes.On;
                ReturnPointList = new Stack<List<DependencyObject>>();
                activeScanList = new List<DependencyObject>();
                windowHistoryPath = new Stack<Window>();
                currentScanIndex = 0;
                //Todo turn on autoscan
            }
            else
            {
                TimerMode = TimerModes.Off;
                scanTimer.Stop();
                ReturnPointList = new Stack<List<DependencyObject>>();
                activeScanList = new List<DependencyObject>();
                //todo turn off autoscan
            }
        }
        
        //TODO add method for closing ActiveWindow
        public void NewActiveWindow(Window newWindow)
        {
            if(TimerMode!=TimerModes.On ) return ;
            if (newWindow == null) return;
            if (windowHistoryPath.Count>0 && newWindow == windowHistoryPath.Peek()) return;

            ReturnPointList = new Stack<List<DependencyObject>>();

            if (currentScanObject != null)
            {
                Application.Current.Dispatcher.Invoke(() => SetIsHighlight(currentScanObject, false));
            }

            var currentWindow = windowHistoryPath.Any() ? windowHistoryPath.Peek() : null;

            var window = currentWindow;
            currentWindow?.Dispatcher.Invoke(() =>
            {
            });

            newWindow.Dispatcher.Invoke(() =>
            {
                newWindow.KeyUp += KeyUp;
                newWindow.KeyDown += KeyDown;
            });
            windowHistoryPath.Push(newWindow);
        }

        /// <summary>
        /// will close active window provided it is not the Original MainWindow
        /// </summary>
        public void CloseActiveWindow()
        {
            if (windowHistoryPath.Count <= 1 || TimerMode == TimerModes.Off) return;
            scanTimer.Stop();
            Window windowToClose = windowHistoryPath.Peek();
//            windowToClose.Close();
            windowHistoryPath.Pop();
            Window newWindow = windowHistoryPath.Peek();
            newWindow.Show();
            scanTimer.Start();
        }


        public void NewListToScanThough(List<DependencyObject> newList, bool isReturnPoint=false)
        {
            if(TimerMode == TimerModes.Off || TimerMode==TimerModes.Paused) return;
            scanTimer.Stop();

            if (currentScanObject != null)
            {
                SetIsHighlight(currentScanObject, false);
            }

            activeScanList = newList ?? throw new NullReferenceException();
            if (isReturnPoint || !ReturnPointList.Any())
            {
                ReturnPointList.Push(newList);
                popReturnPointList = true;
            }
            else
            {
                popReturnPointList = false;
            }

            Direction = DirectionEnum.Forward;
            currentScanIndex = activeScanList.Count-1;
            currentScanObject = activeScanList[currentScanIndex];
            ScanNextItem(null, null);

            scanTimer.Start();
        }

        public void NewListToScanThough<T>(Panel parent, bool isReturnPoint=false) where T : DependencyObject
        {
            if(TimerMode == TimerModes.Off || TimerMode==TimerModes.Paused) return;
            if(parent == null) { throw new NullReferenceException();}

            scanTimer.Stop();
            if (currentScanObject != null)
            {
                SetIsHighlight(currentScanObject, false);
            }
            var tempScanList = ScannableObjectCollector<T>(parent as DependencyObject, new List<DependencyObject>());

            //if xaml is Panel in Panel goes to lowest Level
            while (tempScanList.Count == 1 && tempScanList[0] is Panel newParent)
            {
                tempScanList = ScannableObjectCollector<T>(newParent, new List<DependencyObject>());
                parent = newParent;
            }
            //if after loop the list is empty, then scan the lowest Level For buttons
            if (tempScanList.Count == 0) tempScanList = ScannableObjectCollector<Button>(parent, new List<DependencyObject>());
            
            //if it is still empty then the panel currently has no scannable children stop trying to update activeScanList
            if (tempScanList.Count == 0 ) { scanTimer.Start(); return; }

            activeScanList = tempScanList;
            Direction = GetScanReverse(parent) ? DirectionEnum.Reverse : DirectionEnum.Forward;

            if (!ReturnPointList.Any() || GetIsReturnPoint(parent) || isReturnPoint)
            {
                ReturnPointList.Push(activeScanList);
                popReturnPointList = true;
            }
            else popReturnPointList = false;

            currentScanIndex = activeScanList.Count-1;
            currentScanObject = activeScanList[currentScanIndex];
            ScanNextItem(null, null);

            scanTimer.Start();
        }

        private List<DependencyObject> ScannableObjectCollector<T>(DependencyObject parent, List<DependencyObject> logicalCollection) where T : DependencyObject
        {
            if(TimerMode != TimerModes.On) return null;
            if (parent == null) { return null; }
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (var child in children)
            {
                if ( !(child is DependencyObject depChild) ) continue;

                if (depChild is T)         //searching for type "T" which is usually Button
                {
                    if (depChild is Panel || depChild is Control )
                    {
                        if(GetDoNotScan(depChild)) continue;
                        logicalCollection.Add(depChild);
                    }
                }
                else if (child is Decorator){
                    ScannableObjectCollector<T>(depChild, logicalCollection); //If still in dependencyObject, go into depChild's children
                }
            }
            return logicalCollection;
        }

//todo add Pause Toggle function

        private void ScanNextItem(object sender, ElapsedEventArgs e)
        {
            if(TimerMode == TimerModes.Off || TimerMode==TimerModes.Paused) return;
            windowHistoryPath.Peek().Dispatcher.Invoke(() =>
            {
                if (TimerMode != TimerModes.On) return;
                scanTimer.Stop();
                if (currentScanObject != null)
                {
                    currentScanIndex += (int) Direction;
                    if (activeScanList.Count == 1)
                    {
                        currentScanIndex = 0;
                    }
                    else if (currentScanIndex < 0 || currentScanIndex >= activeScanList.Count)
                    {
                        currentScanIndex -=
                            activeScanList.Count * (int) Direction; // loops the index if it goes negative
                    }

                    SetIsHighlight(currentScanObject, false);
                    //currently highlighted button reverts to original background
                }

                // change to next highlighted button
                currentScanObject = activeScanList[currentScanIndex];

                if (currentScanObject != null)
                {
                    SetIsHighlight(currentScanObject, true);
                    (currentScanObject as Button)?.BringIntoView();
                    (currentScanObject as Panel)?.BringIntoView();
                }

                scanTimer.Start();
            });
        }
        
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
        /// keeps hold event from happening repeatedly
        private bool reverseIsPressed = false; /// keeps hold event from happening repeatedly
        private bool selectIsPressed = false; /// keeps hold event from happening repeatedly
        private bool goBackIsPressed = false;

        /// triggers KeyHold behavior
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(TimerMode==TimerModes.Off) return ;
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse when !reverseIsPressed:
                case (Key)ControlKeys.Reverse2 when !reverseIsPressed:
                    reverseIsPressed = true;
                    ReverseHold?.Invoke(currentScanObject); // equivalent to saying if( ReverseHold != null ){ ReverseHold(); }  //this is a delegate Event for other objects to add methods
                    ReverseHoldDefault();
                    break;
                case (Key)ControlKeys.Select when !selectIsPressed:
                case (Key)ControlKeys.Select2 when !selectIsPressed:
                    selectIsPressed = true;
                    SelectHold?.Invoke(currentScanObject);
                    break;
                case (Key)ControlKeys.GoBack when !goBackIsPressed:
                case (Key)ControlKeys.GoBack2 when !goBackIsPressed:
                    goBackIsPressed = true;
                    GoBackHold?.Invoke(currentScanObject);
                    break;
                default:
                    return;
            }
        }
        // triggers KeyPress behavior
        private void KeyUp(object sender, KeyEventArgs e)
        {
            if(TimerMode==TimerModes.Off) return ;
            var k = e.Key;
            switch (k)
            {
                case (Key)ControlKeys.Reverse :
                case (Key)ControlKeys.Reverse2 :
                    ReversePress?.Invoke(currentScanObject);
                    if(!IgnoreReversePressOnce)ReversePressDefault();
                    IgnoreReversePressOnce = false;
                    reverseIsPressed = false;
                    break;
                case (Key)ControlKeys.Select:
                case (Key)ControlKeys.Select2:
                    SelectPress?.Invoke(currentScanObject);
                    if(!IgnoreSelectPressOnce)SelectPressDefault();
                    IgnoreSelectPressOnce = false;
                    selectIsPressed = false;
                   break;
                case (Key)ControlKeys.GoBack:
                case (Key)ControlKeys.GoBack2:
                    GoBackPress?.Invoke(currentScanObject);
                    //this if statement is used so that the default can be ignored for only one press if set by user
                    if(!IgnoreGoBackPressOnce)GoBackPressDefault();
                    IgnoreGoBackPressOnce = false; 
                    goBackIsPressed = false;
                    break;
                default:
                    return;
            }
        }

        public void GoBackPressDefault()
        {
            if (!GoBackDefaultEnabled) return;
            windowHistoryPath.Peek().Dispatcher.Invoke( () => SetIsHighlight(currentScanObject, false));

            if (TimerMode == TimerModes.Paused)
            {
                TimerMode = TimerModes.On;
                NewListToScanThough(ReturnPointList.Peek());
                popReturnPointList = true;
                return;
            }

            if (popReturnPointList == true)
            {
                if (MainWindow.Instance.Navigator.CanGoBack || ReturnPointList.Count>1)
                {
                    ReturnPointList.Pop();
                }
                if (ReturnPointList.Count > 0) // Path on page not Empty
                {
                    NewListToScanThough(ReturnPointList.Peek());
                    popReturnPointList = true;
                }
                else //Cant go any further Back on CurrentPage
                {
                    switch (windowHistoryPath.Peek())
                    {
                        case MainWindow mw:
                            MainWindow.Instance.Back();
                            break;
                        case SecondaryWindow sw:
                            //CloseActiveWindow();
                            sw.Close();
                            break;
                    }
                }
            }
            else
            {
                NewListToScanThough(ReturnPointList.Peek());
                popReturnPointList = true;
            }
        }


        public void ReversePressDefault()
        {
            if (!ReverseDefaultEnabled) return;
            scanTimer.Stop();
            Direction = Direction == DirectionEnum.Forward ? DirectionEnum.Reverse : DirectionEnum.Forward; 
            scanTimer.Start();
        }

        public void ReverseHoldDefault()
        {
            if (!ReverseDefaultEnabled) return;
            scanTimer.Stop();
            //hei.indexHighlighted -= 1 * FLAG_direction; // go back 2 buttons (after KeyUp autoscanningButtons is called, which adds 1 to index           
            if (currentScanIndex < 0 || currentScanIndex >= activeScanList.Count)
            {
                currentScanIndex += activeScanList.Count * (int)Direction; // loops the index if it goes negative
            }

            Direction = Direction == DirectionEnum.Forward ? DirectionEnum.Reverse : DirectionEnum.Forward; 
            scanTimer.Start();
            ScanNextItem(null, null); //manually calls event handler so the q key press is executed immediately
        }

        public void SelectPressDefault()
        {
            if (!SelectDefaultEnabled) return;
            switch (currentScanObject)
            {
                case Panel panel:
                    //App.Current.Dispatcher.Invoke(() => SetIsHighlight(panel, false));
                    NewListToScanThough<DependencyObject>(panel); //pass in panel that was clicked 
                    break;
                case Button button:
                    if (button is TalkerButton.Button btn && btn.PauseOnSelect)
                    {
                        scanTimer.Stop();
                        TimerMode = TimerModes.Paused;
                        button?.RaiseEvent(new RoutedEventArgs(
                            System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
                    }
                    else
                    {
                        button?.RaiseEvent(new RoutedEventArgs(
                            System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
                        //NewListToScanThough<DependencyObject>(goBackPath.Pop()); //pass in panel that was clicked 
                        if (ReturnPointList.Count > 1)
                        {
                            NewListToScanThough(ReturnPointList.Peek());
                            popReturnPointList = true;
                        }
                        scanTimer.Start();
                    }
                    break;
            }
        }
    }
}
