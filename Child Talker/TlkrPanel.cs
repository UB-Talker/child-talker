using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace Child_Talker
{
    public interface ITlkrPanel
    {
        //the actual variable used to set and get the dependency property
        bool scanReverse 
        {
            get;
            set;
        } 
        
        bool isReturnPoint 
        {
            get;
            set;
        }

        bool DontScan 
        {
            get;
            set;
        } 


    }

    public class TlkrPanel : Panel
    { 
        /* 
         * tells autoscan which direction to scan the panels immediate children 
         * 
         *    true - scan reverse    
         *    false - scan forward  (default)
         */
        public static readonly DependencyProperty scanReverseProperty = DependencyProperty.Register(
            "ScanReverse",                              // Name of property on XAML
            typeof(bool),                               // type of property on XAML
            typeof(TlkrPanel),                    // who inherits this property
            new UIPropertyMetadata(null));              // eventHandler if property changes
        
        /* 
         * when a child UserControl (button or item) is selected return here instead of immediate parent panel 
         * (this is how the keyboard functions) - might not matter if first children are UserControls
         * 
         *    true - return scan to this element     
         *    false - restart current panel after select    (default)
         */
        public static readonly DependencyProperty isReturnPointProperty = DependencyProperty.Register(
            "isReturnPoint",                            // Name of property on XAML
            typeof(bool),                               // type of property on XAML
            typeof(TlkrPanel),                    // who inherits this property
            new UIPropertyMetadata(null));              // eventHandler if property changes

        /*
         * Tells Autoscan whether or not to scan this element 
         *     true - autoscan will ignore this item
         *     false - autoscan continues as normal     (default)
         */
        public static readonly DependencyProperty DontScanProperty = DependencyProperty.Register(
            "DontScan",                             // Name of property on XAML
            typeof(bool),                     // type of property on XAML
            typeof(TlkrPanel),                    // who inherits this property
            new UIPropertyMetadata(null));


        //the actual variable used to set and get the dependency property
        public static bool scanReverse(Panel p)
        { 
            return (bool)p.GetValue(scanReverseProperty);
        }
        public static bool isReturnPoint(Panel p) 
        {
            return (bool)p.GetValue(isReturnPointProperty); 
        }

        public static bool DontScan(Panel p)
        {
            return (bool)p.GetValue(DontScanProperty);
        } 

        public static void addTlkrPanelProperties()
        {
            /*
            scanReverseProperty.AddOwner(typeof(TlkrGrid));
            isReturnPointProperty.AddOwner(typeof(TlkrGrid));
            DontScanProperty.AddOwner(typeof(TlkrGrid));

            scanReverseProperty.AddOwner(typeof(TlkrScrollViewer));
            isReturnPointProperty.AddOwner(typeof(TlkrScrollViewer));
            DontScanProperty.AddOwner(typeof(TlkrScrollViewer));

            scanReverseProperty.AddOwner(typeof(TlkrStackPanel));
            isReturnPointProperty.AddOwner(typeof(TlkrStackPanel));
            DontScanProperty.AddOwner(typeof(TlkrStackPanel));
            */
        }
    }
}
