using System;
using System.Windows;
using System.Windows.Input;

namespace Child_Talker.Utilities.Autoscan
{
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
        public event ScanEventHandler SelectPressOnce;

        /// <summary> occurs once when select is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler SelectHold;
        public event ScanEventHandler SelectHoldOnce;

        /// <summary> occurs when a reverse button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreReversePressOnce = true) </summary>
        public event ScanEventHandler ReversePress;
        public event ScanEventHandler ReversePressOnce;

        /// <summary> occurs once when Reverse is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler ReverseHold;
        public event ScanEventHandler ReverseHoldOnce;
        
        /// <summary> occurs when a GoBack button is pressed before default behavior occurs (to ignore default behavior after occurring set ignoreGoBackPressOnce = true) </summary>
        public event ScanEventHandler GoBackPress;
        public event ScanEventHandler GoBackPressOnce;
        
        /// <summary> occurs once when GoBack is first held down (**may not exist depending on input method) </summary>
        public event ScanEventHandler GoBackHold;
        public event ScanEventHandler GoBackHoldOnce;

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
            ReversePressOnce = null;
            ReverseHoldOnce = null;
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
            GoBackPressOnce = null;
            GoBackHoldOnce = null;
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
            SelectPressOnce = null;
            SelectHoldOnce = null;
            GoBackDefaultEnabled = true;
        }


    }
}
