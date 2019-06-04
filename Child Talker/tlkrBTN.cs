using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private List<DependencyObject> foregroundObjects = new List<DependencyObject>();

        public Brush TkrForeground
        {
            set
            {
                foreach(DependencyObject child in foregroundObjects)
                {
                    if(child is Border) { (child as Border).Background = value;  }
                    else if(child is TextBlock) { (child as TextBlock).Foreground = value;  }
                    if(child is Label) { (child as Label).Foreground = value;  }
                }
            }
            get
            {
                if (foregroundObjects[0] is Border) { return (foregroundObjects[0] as Border).Background; }
                else if (foregroundObjects[0] is TextBlock) { return (foregroundObjects[0] as TextBlock).Foreground; }
                else if (foregroundObjects[0] is Label) { return (foregroundObjects[0] as Label).Foreground; }
                else return Brushes.Black;
            }
        }
        
        

        public TlkrBTN()
        {
            this.Initialized += initSetup;
        }

        private void initSetup(object sender, EventArgs e)
        {
            foregroundCollection(sender as TlkrBTN, foregroundObjects);
        }

        private static void foregroundCollection(DependencyObject parent, List<DependencyObject> logicalCollection)
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (object child in children)
            {
                if (child == null) { break; }
                if (child is Border) { logicalCollection.Add(child as Border); }
                else if (child is TextBlock) { logicalCollection.Add(child as TextBlock); }
                else if (child is Label) { logicalCollection.Add(child as Label); }
                else
                {
                    DependencyObject depChild = child as DependencyObject;
                    foregroundCollection(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                }
            }
        }
    }

}
