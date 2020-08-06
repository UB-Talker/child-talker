using Child_Talker.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Child_Talker.Utilities.HardwareIntegrations;
using SysButton = System.Windows.Controls.Button;
using Timer = System.Timers.Timer;

namespace Child_Talker.Utilities.Autoscan
{
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

        public Window ActiveWindow => windowHistory.Last();


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
        
        public void ManualScan(bool toggle)
        {
            if (TimerMode == TimerModes.Off) return;
            if (toggle)
            {
                scanTimer.Stop();
                TimerMode = TimerModes.Manual;
                HighlightNext();
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

            _ = windowHistory.AddLast(newWindow);
            KeyboardIntegration.Instance.ApplyIntegration(newWindow);
        }

        /// <summary>
        /// will close active window provided it is not the Original MainWindow
        /// </summary>
        public void CloseActiveWindow(Window closeThis)
        {
            if (windowHistory.Count <= 1 || TimerMode == TimerModes.Off) return;
            ResetEventHandlers();
            _ = windowHistory.Remove(closeThis);
            var newWindow = windowHistory.Last();
            newWindow.Show();
        }

        /// <summary>
        /// Changes <see cref="activeScanList"/>. Will scan though all elements from the provided list
        /// </summary>
        /// <param name="newList"> a list of all elements to scan through</param>
        /// <param name="isReturnPoint"> when true adds newList to the top of <see cref="ReturnPointList"/></param>
        public void NewListToScanThough(List<DependencyObject> newList, bool isReturnPoint = false, bool manualScan=false)
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
            ManualScan(manualScan);
            currentScanIndex = activeScanList.Count - 1;
            currentScanObject = activeScanList[currentScanIndex];
            ScanTimerElapsed(null, null);

            scanTimer.Start();
        }

        private Panel Parent;

        public void rescan<T>() where T : DependencyObject
        {
            NewListToScanThough<T>(Parent);

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
            _ = windowHistory.Last().Focus();
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

            Parent = parent;
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
            ManualScan(GetManualScan(parent));
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
                            _ = ScannableObjectCollector<T>(depChild, logicalCollection);
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
                    _ = ScannableObjectCollector<T>(depChild, logicalCollection); //If still in dependencyObject, go into depChild's children
                }
            }

            return logicalCollection;
        }

        /// <summary>
        /// Functionality for autoscan to move to the next element while scanning
        /// </summary>
        private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
        { 
            if (TimerMode != TimerModes.On) return;
            scanTimer.Stop();
            HighlightNext();
            scanTimer.Start();
        }
        private void HighlightNext()
        {
            if (TimerMode == TimerModes.On || TimerMode == TimerModes.Manual) { } else { return;}
            if (activeScanList.Count == 0)
            {
                throw new Exception("Autoscan List is empty");
            }
            
            //this MUST throw an error window history should never contain less than 1 element (MainWindow)
            windowHistory.Last().Dispatcher.Invoke(() =>
            {
                if (TimerMode == TimerModes.On || TimerMode == TimerModes.Manual) { } else { return;}
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

            });
        }

    }
}