using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using SysButton = System.Windows.Controls.Button;
using Button = Child_Talker.TalkerButton.Button;
using System.Windows.Input;
using Child_Talker.Properties;
using Child_Talker.TalkerViews;
using Timer = System.Timers.Timer;

namespace Child_Talker.Utilities
{

    /// <summary>
    /// this section is being used for Attached DependencyProperties
    /// and RoutedCommands
    /// </summary>
    public partial class Autoscan2
    {

        /// <summary>
        /// tells autoscan which direction to scan the panels immediate children 
        /// <para>    true - scan reverse             </para>
        /// <para>   false - scan forward  (default)  </para>
        /// </summary>
        public static readonly DependencyProperty ScanReverseProperty = DependencyProperty.RegisterAttached(
            "ScanReverse", // Name of property on XAML
            typeof(bool), // type of property on XAML
            typeof(Autoscan2), // who inherits this property
            new UIPropertyMetadata(null)); // eventHandler if property changes

        /// <summary>
        /// when a child UserControl (button or item) is selected return here instead of immediate parent panel 
        /// (this is how the keyboard functions) - might not matter if first children are UserControls
        /// <para>   true - return scan to this element     </para>
        /// <para>   false - restart current panel after select    (default)</para>
        /// </summary>
        public static readonly DependencyProperty IsReturnPointProperty = DependencyProperty.RegisterAttached(
            "IsReturnPoint", // Name of property on XAML
            typeof(bool), // type of property on XAML
            typeof(Autoscan2), // who inherits this property
            new UIPropertyMetadata(null)); // eventHandler if property changes

        /// <summary>
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item              </para>
        /// <para>    false - autoscan continues as normal     (default) </para>
        /// </summary>
        public static readonly DependencyProperty DoNotScanProperty = DependencyProperty.RegisterAttached(
            "DoNotScan", // Name of property on XAML
            typeof(bool), // type of property on XAML
            typeof(Autoscan2), // who inherits this property
            new UIPropertyMetadata(false)); // eventHandler if property changes

        //the actual variable used to set and get the dependency property
        public static bool GetScanReverse(DependencyObject obj)
        {
            return (bool) obj.GetValue(ScanReverseProperty);
        }

        public static void SetScanReverse(DependencyObject obj, bool value)
        {
            obj.SetValue(ScanReverseProperty, value);
        }

        public static bool GetIsReturnPoint(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsReturnPointProperty);
        }

        public static void SetIsReturnPoint(DependencyObject obj, bool value)
        {
            obj.SetValue(ScanReverseProperty, value);
        }

        public static bool GetDoNotScan(DependencyObject obj)
        {
            return (bool) obj.GetValue(DoNotScanProperty);
        }

        public static void SetDoNotScan(DependencyObject obj, bool value)
        {
            obj.SetValue(DoNotScanProperty, value);
        }

        public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.RegisterAttached(
            "IsHighlight", // Name of property on XAML
            typeof(bool), // type of property on XAML
            typeof(Autoscan2), // who inherits this property
            new UIPropertyMetadata(false)); // eventHandler if property changes

        public static bool GetIsHighlight(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsHighlightProperty);
        }

        public static void SetIsHighlight(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHighlightProperty, value);
        }
    }

    /// <summary>
    /// This section is used for EventHandling
    /// </summary>
    public partial class Autoscan2
    {
        public delegate void DefaultEvents();
        //delegates are used to define parameter and return value for event listener
        /// <summary>
        /// Event Handler type for all Autoscan Events passes a reference to the currently highlighted object
        /// <para> currentObject - The element being highlighted when this event was triggered </para>
        /// <para> defaultEvent - The default event associated with the designated trigger </para>
        /// </summary>
        /// <param name="currentObject"> The element being highlighted when this event was triggered </param>
        /// <param name="defaultEvent"> The default event associated with the designated trigger </param>
        public delegate void ScanEventHandler(DependencyObject currentObject, DefaultEvents defaultEvent = null);
        
        /// <summary> occurs when a select button is pressed before default behavior occurs (to disable default behavior just once set ignoreSelectPressOnce = true) </summary>
        public event ScanEventHandler SelectPress;
        /// <summary> occurs once when select is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler SelectHold;
        /// <summary> occurs when a reverse button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreReversePressOnce = true) </summary>
        public event ScanEventHandler ReversePress;
        /// <summary> occurs once when Reverse is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler ReverseHold;
        /// <summary> occurs when a GoBack button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreGoBackPressOnce = true) </summary>
        public event ScanEventHandler GoBackPress;
        /// <summary> occurs once when GoBack is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler GoBackHold;

        /// <summary>
        /// Resets all  <see cref="Autoscan2"/> EventHandlers
        /// And Re-enables Default behavior
        /// </summary>
        public void ResetEventHandlers()
        {
            ResetGoBackEventHandlers();
            ResetReverseEventHandlers();
            ResetSelectEventHandlers();
        }
        /// <summary>
        /// Resets <see cref="Autoscan2"/> Reverse EventHandlers
        /// And Re-enables Default behavior
        /// </summary>
        public void ResetReverseEventHandlers()
        {
            ReversePress = null;
            ReverseHold = null;
            ReverseDefaultEnabled = true;
        }
        /// <summary>
        /// Resets <see cref="Autoscan2"/> GoBack EventHandlers
        /// And Re-enables Default behavior
        /// </summary>
        public void ResetGoBackEventHandlers()
        {
            GoBackPress = null;
            GoBackHold = null;
            GoBackDefaultEnabled = true;
        }
        /// <summary>
        /// Resets <see cref="Autoscan2"/> Select EventHandlers
        /// And Re-enables Default behavior
        /// </summary>
        public void ResetSelectEventHandlers()
        {
            SelectPress = null;
            SelectHold = null;
            GoBackDefaultEnabled = true;
        }
    }

    public partial class Autoscan2
    {
        private static Autoscan2 _instance;
        public static Autoscan2 Instance => _instance ?? (_instance = new Autoscan2());

        private Autoscan2()
        {
            scanTimer.Elapsed += ScanTimerElapsed;
            scanTimer.Interval = Properties.AutoscanSettings.Default.scanSpeed;

            Properties.AutoscanSettings.Default.SettingChanging += SettingsChanged;
            bool autoscanEnabled = Properties.AutoscanSettings.Default.Enabled;
            if (!autoscanEnabled) return;
            TimerMode = TimerModes.On;
            ReturnPointList = new Stack<List<DependencyObject>>();
            activeScanList = new List<DependencyObject>();
            currentScanIndex = 0;
        }

        /// <summary>
        /// getter/setter for scanTimer.Interval
        /// </summary>
        public double ScanTimerInterval
        {
            get => scanTimer.Interval;
            private set
            {
                scanTimer.Stop();
                scanTimer.Interval = value;
                if(TimerMode == TimerModes.On) scanTimer.Start();
            }
        }
        /// <summary>
        /// Triggers if any autoscan related settings are changed
        /// <para> see <see cref="AutoscanSettings.settings"/> for all applicable settings</para>
        /// </summary>
        private void SettingsChanged(object sender, SettingChangingEventArgs e)
        {
            if(e.SettingName.Equals("scanSpeed"))
                ScanTimerInterval = (double)e.NewValue;
        }
        /// <summary>
        /// When Elapsed Autoscan will highlight and set triggers on next Element in <see cref="activeScanList"/>
        /// <para>see <see cref="ScanTimerElapsed"/> for all functionality</para> 
        /// </summary>
        private readonly Timer scanTimer = new Timer();
        /// <summary>
        /// When a panel has <see cref="Autoscan2.IsReturnPointProperty"/> set to true, the <see cref="activeScanList"/> is added to the stack.
        /// This stack Create the order of return when <see cref="GoBackPressDefault"/> occurs
        ///<para>
        /// -- !!! -- MUST be set to true when scanning a new page. (this is done by default in both MainWindow and SecondaryWindow)
        /// </para>
        /// </summary>
        public Stack<List<DependencyObject>> ReturnPointList { get; private set; } = new Stack<List<DependencyObject>>();
        /// <summary>
        /// Determines what to scan next when <see cref="GoBackPressDefault"/> occurs.
        /// <para> When TRUE, the Panel currently being scanned through is at the top <see cref="ReturnPointList"/>.
        ///        If True pop off the top of ReturnPointList and begin scanning through the new Top.
        /// </para>
        /// <para> When FALSE, the panel is currently scanning in a Panel that is a child of a Panel in <see cref="ReturnPointList"/>.
        ///        If False, GoBack will scan the current Top in <see cref="ReturnPointList"/>
        /// </para>
        /// </summary>
        private bool popReturnPointList = false;
        /// <summary>
        /// Clears all returnPoints for current page 
        /// </summary>
        /// must occur when the contents that were being scanned through are no longer visible or when the window in focus is changed
        public void ClearReturnPointList()
        {
            ReturnPointList = new Stack<List<DependencyObject>>();
        }
        /// <summary>
        /// Keeps track of all open or hidden windows that 
        /// </summary>
        private readonly LinkedList<Window> windowHistory = new LinkedList<Window>();

        /// <summary>
        /// List of all elements currently being scanned through
        /// </summary>
        // TODO would a linked list make more sense?
        private List<DependencyObject> activeScanList;
        /// <summary>
        /// a Reference to the currently highlighted object
        /// </summary>
        private DependencyObject currentScanObject;
        /// <summary>
        /// Index on <see cref="currentScanObject"/> within <see cref="activeScanList"/>
        /// </summary>
        private int currentScanIndex = 0;
        /// List of Keyboard Keys that are used for autoscan
        // TODO find a better way to list all potential trigger events this is inefficient. Learn about RoutedCommands as one possibility
        private enum ControlKeys
        {
            GoBack = Key.S,
            Reverse = Key.Q,
            Select = Key.E,
            GoBack2 = Key.I,
            Reverse2 = Key.J,
            Select2 = Key.L
        }

        /// <summary>
        /// Pauses <see cref="scanTimer"/>. User input will still register but the highlighted object will not change again until <see cref="GoBackPressDefault"/> Occurs.
        /// </summary>
        /// <param name="toggle"></param>
        public void PauseScan(bool toggle)
        {
            if (TimerMode == TimerModes.Off) return;
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
        /// How autoscan knows if it is currently running scanning is paused or autoscan is disabled
        /// </summary>
        /// <para> off/on - says Autoscan is either on or off  </para>
        /// <para> Manual - Timer is Disabled, Movement can still occur through Reverse actions </para>
        /// <para> Paused - Timer and ReversePress are both Disabled but SelectPress and GoBackPress Function normally </para>
        public enum TimerModes
        {
            /// says Autoscan is off  
            Off = 0,
            /// says Autoscan is on
            On = 1,
            /// Movement can only occur by through ReversePress ,Timer is Disabled. SelectPress and GoBackPress Function normally
            Manual = 2,
            /// Timer and ReversePress are both Disabled but SelectPress and GoBackPress Function normally 
            Paused = 3,
        }
        /// <summary> See <see cref="TimerModes"/> for more information </summary>
        public TimerModes TimerMode { get; private set; } = 0;

        /// <summary>
        /// Autoscan Direction
        /// <para> Forward - Usually will scan Left to Right and Top to Bottom</para>
        /// <para> Reverse - Usually will scan Right to Left  and Bottom to Top</para>
        /// </summary>
        public enum DirectionEnum
        {
            /// <summary> Usually will scan Left to Right and Top to Bottom </summary>
            Forward = 1,
            /// <summary> Usually will scan Right to Left  and Bottom to Top </summary>
            Reverse = -1
        };
        /// <summary> See <see cref="DirectionEnum"/> for more information </summary>
        public DirectionEnum Direction { get; private set; } = DirectionEnum.Forward;
        
        /// <summary>
        ///  Sets <see cref="TimerMode"/> to either On or off and saves the current value in <see cref="AutoscanSettings"/>
        /// </summary>
        public void ToggleAutoscan()
        {
            if (TimerMode == TimerModes.Off)
            {
                TimerMode = TimerModes.On;
                ReturnPointList = new Stack<List<DependencyObject>>();
                activeScanList = new List<DependencyObject>();
                currentScanIndex = 0;
                //UpdateScanTimerInterval();
                Properties.AutoscanSettings.Default.Enabled = true;
            }
            else
            {
                if (currentScanObject != null) SetIsHighlight(currentScanObject, false);
                TimerMode = TimerModes.Off;
                scanTimer.Stop();
                ReturnPointList = new Stack<List<DependencyObject>>();
                activeScanList = new List<DependencyObject>();
                Properties.AutoscanSettings.Default.Enabled = false;
            }
            Properties.AutoscanSettings.Default.Save();
        }
        
        /// <summary>
        /// When a new Window is created it must be provided with the Autoscan Event Listeners
        /// </summary>
        /// <param name="newWindow"></param>
        public void NewWindow(Window newWindow)
        {
            if (newWindow == null) return;
            if (windowHistory.Count > 1 && TimerMode == TimerModes.Off) return;
            if (windowHistory.Count > 0 && newWindow == windowHistory.Last()) return;
            if (windowHistory.Contains(newWindow)) return;

            newWindow.Dispatcher?.Invoke(() =>
            {
                newWindow.KeyUp += KeyUp;
                newWindow.KeyDown += KeyDown;
            });
            windowHistory.AddLast(newWindow);
        }

        /// <summary>
        /// will close active window provided it is not the Original MainWindow
        /// </summary>
        public void CloseActiveWindow(Window closeThis)
        {
            if (windowHistory.Count <= 1 || TimerMode == TimerModes.Off) return;
            ResetEventHandlers();
            windowHistory.Remove(closeThis);
            var newWindow = windowHistory.Last();
            newWindow.Show();
        }

        /// <summary>
        /// Changes <see cref="activeScanList"/>. Will scan though all elements from the provided list
        /// </summary>
        /// <param name="newList"> a list of all elements to scan through</param>
        /// <param name="isReturnPoint"> when true adds newList to the top of <see cref="ReturnPointList"/></param>
        public void NewListToScanThough(List<DependencyObject> newList, bool isReturnPoint = false)
        {
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            scanTimer.Stop();
            
            if (currentScanObject != null) SetIsHighlight(currentScanObject, false);

            if(newList.Count == 0) throw  new NullReferenceException("newList to autoscan through is empty");
            activeScanList = newList ?? throw new NullReferenceException("New list to autoscan through does not exists");
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
            currentScanIndex = activeScanList.Count - 1;
            currentScanObject = activeScanList[currentScanIndex];
            ScanTimerElapsed(null, null);

            scanTimer.Start();
        }
        /// <summary>
        /// Changes <see cref="activeScanList"/>. Will scan all immediate Children of type T from the provided Panel parent
        /// </summary>
        /// <typeparam name="T"> what type of <see cref="DependencyObject"/> to add to active scan list. (when set to Dependency object all scannable children will be added to <see cref="activeScanList"/>) </typeparam>
        /// <param name="parent"> will search through <see cref="Panel"/> to find all Children of type <see cref="T"/> </param>
        /// <param name="isReturnPoint"> when true adds newList to the top of <see cref="ReturnPointList"/></param>
        public void NewListToScanThough<T>(Panel parent, bool isReturnPoint = false) where T : DependencyObject
        {
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            if (parent == null) throw new NullReferenceException();
            scanTimer.Stop();
            windowHistory.Last().Focus();
            if (currentScanObject != null) SetIsHighlight(currentScanObject, false);

            var tempScanList = ScannableObjectCollector<T>(parent as DependencyObject, new List<DependencyObject>());

            //if xaml is Panel in Panel goes to lowest Level
            while (tempScanList.Count == 1 && tempScanList[0] is Panel newParent)
            {
                tempScanList = ScannableObjectCollector<T>(newParent, new List<DependencyObject>());
                parent = newParent;
            }

            //if after loop the list is empty, then scan the lowest Level For buttons
            if (tempScanList.Count == 0)
                tempScanList = ScannableObjectCollector<SysButton>(parent, new List<DependencyObject>());

            //if it is still empty then the panel currently has no scannable children stop trying to update activeScanList
            if (tempScanList.Count == 0)
            {
                scanTimer.Start();
                return;
            }

            activeScanList = tempScanList;
            Direction = GetScanReverse(parent) ? DirectionEnum.Reverse : DirectionEnum.Forward;

            if (!ReturnPointList.Any() || GetIsReturnPoint(parent) || isReturnPoint)
            {
                ReturnPointList.Push(activeScanList);
                popReturnPointList = true;
            }
            else
            {
                popReturnPointList = false;
            }

            currentScanIndex = activeScanList.Count - 1;
            currentScanObject = activeScanList[currentScanIndex];
            ScanTimerElapsed(null, null);

            scanTimer.Start();
        }
        
        /// <summary>
        /// recursive function. all elements of type <see cref="T"/> that exist within <see cref="parent"/> will be added to the end of <see cref="logicalCollection"/>
        /// </summary>
        /// <typeparam name="T"> the type of <see cref="DependencyObject"/> being searched for </typeparam>
        /// <param name="parent"> <see cref="Panel"/> currently being searched through </param>
        /// <param name="logicalCollection"> scannable elements are added at the end of logicalCollection</param>
        /// <returns><see cref="logicalCollection"/></returns>
        private List<DependencyObject> ScannableObjectCollector<T>(DependencyObject parent, List<DependencyObject> logicalCollection = null) where T : DependencyObject
        {
            if (TimerMode != TimerModes.On) return null;
            if (parent == null) return null;
            if (logicalCollection == null) logicalCollection = new List<DependencyObject>();
            var children = LogicalTreeHelper.GetChildren(parent);

            foreach (var child in children)
            {
                if (!(child is DependencyObject depChild)) continue;

                if (depChild is T) //searching for type "T" which is usually Button
                {
                    if (depChild is Panel || depChild is Control)
                    {
                        if (depChild is ScrollViewer sv)
                        {
                            if (sv.Content is StackPanel sp) sp.ScrollOwner = sv;
                            ScannableObjectCollector<T>(depChild, logicalCollection);
                            continue;
                        }
                        // a Control must be both enabled and visible to be scanned
                        if (depChild is Control c)
                        {
                            if(!c.IsEnabled) continue;
                            if(c.Visibility != Visibility.Visible) continue;
                        }
                        if (GetDoNotScan(depChild)) continue;

                        logicalCollection.Add(depChild);
                    }
                }
                else if (child is Decorator)
                {
                    ScannableObjectCollector<T>(depChild, logicalCollection); //If still in dependencyObject, go into depChild's children
                }
            }

            return logicalCollection;
        }

        /// <summary>
        /// Functionality for autoscan to move to the next element while scanning
        /// </summary>
        private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            if (activeScanList.Count == 0)
            {
                throw new Exception("Autoscan List is empty");
            }
            
            //this MUST throw an error window history should never contain less than 1 element (MainWindow)
            windowHistory.Last().Dispatcher.Invoke(() =>
            {
                if (TimerMode != TimerModes.On) return;
                scanTimer.Stop();
                if (currentScanObject != null)
                {
                    currentScanIndex += (int) Direction;
                    if (activeScanList.Count == 1)
                        currentScanIndex = 0;
                    else if (currentScanIndex < 0 || currentScanIndex >= activeScanList.Count)
                        currentScanIndex -=
                            activeScanList.Count * (int) Direction; // loops the index if it goes negative

                    SetIsHighlight(currentScanObject, false);
                    //currently highlighted button reverts to original background
                }

                // change to next highlighted button
                currentScanObject = activeScanList[currentScanIndex];

                if (currentScanObject != null)
                {
                    SetIsHighlight(currentScanObject, true);
                    (currentScanObject as SysButton)?.BringIntoView();
                    (currentScanObject as Panel)?.BringIntoView();
                }

                scanTimer.Start();
            });
        }

        /// <summary>
        /// Disables default behavior until manually enabled using <see cref="ResetEventHandlers"/>
        /// <para> ** Use of <c>IgnoreSelectPressOnce</c> instead is highly recommended </para></summary>
        public bool SelectDefaultEnabled = true;
        /// <summary> Disables default behavior until manually enabled using <see cref="resetReverseHandlers"/>
        /// <para> ** Use of <c>IgnoreReversePressOnce</c> instead is highly recommended </para> </summary>
        public bool ReverseDefaultEnabled = true;
        /// <summary> Disables default behavior until manually enabled using <see cref="resetGoBackHandlers"/>
        /// <para> ** Use of <c>IgnoreGoBackPressOnce</c> instead is highly recommended </para></summary>
        public bool GoBackDefaultEnabled = true;
        /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreReversePressOnce = false;
        /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreSelectPressOnce = false;
        ///  If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreGoBackPressOnce = false;

        /// keeps hold event from happening repeatedly
        private bool reverseIsPressed = false;
        /// keeps hold event from happening repeatedly
        private bool selectIsPressed = false;
        /// keeps hold event from happening repeatedly
        private bool goBackIsPressed = false;

        /// triggers KeyHold behavior
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (TimerMode == TimerModes.Off) return;
            var k = e.Key;
            switch (k)
            {
                case (Key) ControlKeys.Reverse when !reverseIsPressed:
                case (Key) ControlKeys.Reverse2 when !reverseIsPressed:
                    reverseIsPressed = true;
                    ReverseHold?.Invoke(currentScanObject, ReversePressDefault); // equivalent to saying if( ReverseHold != null ){ ReverseHold(); }  //this is a delegate Event for other objects to add methods
                    ReverseHoldDefault();
                    break;
                case (Key) ControlKeys.Select when !selectIsPressed:
                case (Key) ControlKeys.Select2 when !selectIsPressed:
                    selectIsPressed = true;
                    SelectHold?.Invoke(currentScanObject);
                    break;
                case (Key) ControlKeys.GoBack when !goBackIsPressed:
                case (Key) ControlKeys.GoBack2 when !goBackIsPressed:
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
            if (TimerMode == TimerModes.Off) return;
            var k = e.Key;
            switch (k)
            {
                case (Key) ControlKeys.Reverse:
                case (Key) ControlKeys.Reverse2:
                    ReversePress?.Invoke(currentScanObject, ReversePressDefault);
                    if (!IgnoreReversePressOnce) ReversePressDefault();
                    IgnoreReversePressOnce = false;
                    reverseIsPressed = false;
                    break;
                case (Key) ControlKeys.Select:
                case (Key) ControlKeys.Select2:
                    SelectPress?.Invoke(currentScanObject, SelectPressDefault);
                    if (!IgnoreSelectPressOnce) SelectPressDefault();
                    IgnoreSelectPressOnce = false;
                    selectIsPressed = false;
                    break;
                case (Key) ControlKeys.GoBack:
                case (Key) ControlKeys.GoBack2:
                    GoBackPress?.Invoke(currentScanObject, GoBackPressDefault);
                    //this if statement is used so that the default can be ignored for only one press if set by user
                    if (!IgnoreGoBackPressOnce) GoBackPressDefault();
                    IgnoreGoBackPressOnce = false;
                    goBackIsPressed = false;
                    break;
                default:
                    return;
            }
        }

        public void GoBackPressDefault()
        {
            if (TimerMode == TimerModes.Off) return;
            if (!GoBackDefaultEnabled) return;
            goBackIsPressed = true;
            windowHistory.Last().Dispatcher.Invoke(() => SetIsHighlight(currentScanObject, false));

            if (TimerMode == TimerModes.Paused)
            {
                TimerMode = TimerModes.On;
                NewListToScanThough(ReturnPointList.Peek());
                popReturnPointList = true;
                return;
            }

            if (popReturnPointList == true)
            {
                if (MainWindow.Instance.Navigator.CanGoBack || ReturnPointList.Count > 1) ReturnPointList.Pop();
                if (ReturnPointList.Count > 0) // Path on page not Empty
                {
                    NewListToScanThough(ReturnPointList.Peek());
                    popReturnPointList = true;
                }
                else //Cant go any further Back on CurrentPage
                {
                    switch (windowHistory.Last())
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
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            if (!ReverseDefaultEnabled) return;
            reverseIsPressed = true;
            scanTimer.Stop();
            Direction = Direction == DirectionEnum.Forward ? DirectionEnum.Reverse : DirectionEnum.Forward;
            scanTimer.Start();
        }

        public void ReverseHoldDefault()
        {
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            if (!ReverseDefaultEnabled) return;
            scanTimer.Stop();
            //hei.indexHighlighted -= 1 * FLAG_direction; // go back 2 buttons (after KeyUp autoscanningButtons is called, which adds 1 to index           
            if (currentScanIndex < 0 || currentScanIndex >= activeScanList.Count)
                currentScanIndex += activeScanList.Count * (int) Direction; // loops the index if it goes negative

            Direction = Direction == DirectionEnum.Forward ? DirectionEnum.Reverse : DirectionEnum.Forward;
            scanTimer.Start();
            ScanTimerElapsed(null, null); //manually calls event handler so the q key press is executed immediately
        }

        public void SelectPressDefault()
        {
            if (TimerMode == TimerModes.Off) return;
            if (!SelectDefaultEnabled) return;
            selectIsPressed = true;
            switch (currentScanObject)
            {
                case Panel panel:
                    //App.Current.Dispatcher.Invoke(() => SetIsHighlight(panel, false));
                    NewListToScanThough<DependencyObject>(panel); //pass in panel that was clicked 
                    break;
                case ScrollViewer scrollViewer:
                    NewListToScanThough<DependencyObject>(scrollViewer.Content as Panel); //pass in panel that was clicked 
                    break;
                case SysButton button:
                    if (button is TalkerButton.Button btn && btn.PauseOnSelect)
                    {
                        scanTimer.Stop();
                        TimerMode = TimerModes.Paused;
                        btn?.RaiseEvent(new RoutedEventArgs(
                            System.Windows.Controls.Primitives.ButtonBase
                                .ClickEvent)); // how you simulate a button click in code
                    }
                    else
                    {
                        button?.RaiseEvent(new RoutedEventArgs(
                            System.Windows.Controls.Primitives.ButtonBase
                                .ClickEvent)); // how you simulate a button click in code
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