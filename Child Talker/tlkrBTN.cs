using System.Windows;
using System.Windows.Controls;

namespace Child_Talker
{
    public class TlkrBTN : Button 
    {
        /*
         *  when this Button is selected the autoscan stops cycling until 'go back' is pressed
         *     true - pause autoscan
         *     false - autoscan continues as normal     (default)
         */
        public static readonly DependencyProperty pauseOnSelectProperty = DependencyProperty.Register(
            "PauseOnSelect",                        // Name of property on XAML
            typeof(bool),                           // type of property on XAML
            typeof(TlkrBTN),                        // who inherits this property
            new UIPropertyMetadata(null));

        /*
         * Tells Autoscan whether or not to scan this element 
         *     true - autoscan will ignore this item
         *     false - autoscan continues as normal     (default)
         */
        public static readonly DependencyProperty DontScanProperty = DependencyProperty.Register(
            "DontScan",                             // Name of property on XAML
            typeof(bool),                           // type of property on XAML
            typeof(TlkrBTN),                        // who inherits this property
            new UIPropertyMetadata(null));

        public bool PauseOnSelect
        {
            get { return (bool)GetValue(pauseOnSelectProperty); }
            set { SetValue(pauseOnSelectProperty, value); }
        } 
        
        public bool DontScan 
        {
            get { return (bool)GetValue(DontScanProperty); }
            set { SetValue(DontScanProperty, value); }
        } 
    }

}
