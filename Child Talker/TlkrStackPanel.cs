using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Child_Talker
{
    public class TlkrStackPanel : StackPanel
    { 
        //the actual variable used to set and get the dependency property
        public bool scanReverse 
        {
            get { return (bool)GetValue(TlkrPanel.scanReverseProperty); }
            set { SetValue(TlkrPanel.scanReverseProperty, value); }
        } 
        
        public bool isReturnPoint 
        {
            get { return (bool)GetValue(TlkrPanel.isReturnPointProperty); }
            set { SetValue(TlkrPanel.isReturnPointProperty, value);
            }
        }

        public bool DontScan 
        {
            get { return (bool)GetValue(TlkrPanel.DontScanProperty); }
            set { SetValue(TlkrPanel.DontScanProperty, value); }
        } 

    }
}
