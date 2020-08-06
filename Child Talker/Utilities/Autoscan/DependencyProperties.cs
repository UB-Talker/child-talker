using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Child_Talker.Utilities.Autoscan
{/// <summary>
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

        public static bool GetScanReverse(DependencyObject obj)
        {
            return (bool) obj.GetValue(ScanReverseProperty);
        }

        public static void SetScanReverse(DependencyObject obj, bool value)
        {
            obj.SetValue(ScanReverseProperty, value);
        }

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

        public static bool GetIsReturnPoint(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsReturnPointProperty);
        }

        public static void SetIsReturnPoint(DependencyObject obj, bool value)
        {
            obj.SetValue(ScanReverseProperty, value);
        }

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
        public static bool GetDoNotScan(DependencyObject obj)
        {
            return (bool) obj.GetValue(DoNotScanProperty);
        }

        public static void SetDoNotScan(DependencyObject obj, bool value)
        {
            obj.SetValue(DoNotScanProperty, value);
        }
        
        /// <summary>
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item              </para>
        /// <para>    false - autoscan continues as normal     (default) </para>
        /// </summary>
        public static readonly DependencyProperty ManualScanProperty = DependencyProperty.RegisterAttached(
            "ManualScan", // Name of property on XAML
            typeof(bool), // type of property on XAML
            typeof(Autoscan2), // who inherits this property
            new UIPropertyMetadata(false)); // eventHandler if property changes
        public static bool GetManualScan(DependencyObject obj)
        {
            return (bool) obj.GetValue(ManualScanProperty);
        }

        public static void SetManualScan(DependencyObject obj, bool value)
        {
            obj.SetValue(ManualScanProperty, value);
        }

        /// <summary>
        /// Is used for stylizing purposes.
        /// should only be true for the <see cref="Autoscan2.currentScanObject"/>
        /// </summary>
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
}
