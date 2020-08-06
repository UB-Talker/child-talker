using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SysButton = System.Windows.Controls.Button;

namespace Child_Talker.Utilities.Autoscan
{
    public partial class Autoscan2
    {
        public enum ControlOptions
        {
            GoBack,
            Reverse,
            Select
        }

        /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreReversePressOnce = false;
        /// If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreSelectPressOnce = false;
        ///  If set to true, will Skip Default behavior once immediately after Invocation is called
        public bool IgnoreGoBackPressOnce = false;

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

        public void GoBackPressDefault()
        {
            if (TimerMode == TimerModes.Off) return;
            if (!GoBackDefaultEnabled) return;
            windowHistory.Last().Dispatcher.Invoke(() => SetIsHighlight(currentScanObject, false));

            if (TimerMode == TimerModes.Paused || TimerMode == TimerModes.Manual)
            {
                TimerMode = TimerModes.On;
                NewListToScanThough(ReturnPointList.Peek());
                popReturnPointList = true;
                return;
            }

            if (popReturnPointList == true)
            {
                if (MainWindow.Instance.Navigator.CanGoBack || ReturnPointList.Count > 1) _ = ReturnPointList.Pop();
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
            if (TimerMode == TimerModes.Manual)
            {
                HighlightNext();
                return;
            }
            if (TimerMode == TimerModes.Off || TimerMode == TimerModes.Paused) return;
            if (!ReverseDefaultEnabled) return;
            scanTimer.Stop();
            Direction = Direction == DirectionEnum.Forward ? DirectionEnum.Reverse : DirectionEnum.Forward;
            scanTimer.Start();
        }

        public void ReverseHoldDefault()
        {
            if (TimerMode != TimerModes.On) return;
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
            switch (currentScanObject)
            {
                case Panel panel:
                    //App.Current.Dispatcher.Invoke(() => SetIsHighlight(panel, false));
                    NewListToScanThough<DependencyObject>(panel); //pass in panel that was clicked 
                    break;
                case ScrollViewer scrollViewer:
                    NewListToScanThough<DependencyObject>(
                        scrollViewer.Content as Panel); //pass in panel that was clicked 
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
        
        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void ReversePressIntegration()
        {
            ReversePress?.Invoke(currentScanObject, ReversePressDefault);
            if (!IgnoreReversePressOnce) ReversePressDefault();
            IgnoreReversePressOnce = false;
        }

        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void SelectPressIntegration()
        {
            SelectPress?.Invoke(currentScanObject, SelectPressDefault);
            if (!IgnoreSelectPressOnce) SelectPressDefault();
            IgnoreSelectPressOnce = false;
        }

        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void GoBackPressIntegration()
        {
            GoBackPress?.Invoke(currentScanObject, GoBackPressDefault);
            //this if statement is used so that the default can be ignored for only one press if set by user
            if (!IgnoreGoBackPressOnce) GoBackPressDefault();
            IgnoreGoBackPressOnce = false;
        }

        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void ReverseHoldIntegration()
        {
            ReverseHold?.Invoke(currentScanObject, ReversePressDefault); // equivalent to saying if( ReverseHold != null ){ ReverseHold(); }  //this is a delegate Event for other objects to add methods
            ReverseHoldDefault();
        }

        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void SelectHoldIntegration()
        {
            SelectHold?.Invoke(currentScanObject);
        }

        /// <summary>
        /// called from a Hardware integration class to provide designated functionality
        /// </summary>
        public void GoBackHoldIntegration()
        {
            GoBackHold?.Invoke(currentScanObject);
        }
    }
}
