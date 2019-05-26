using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Child_Talker
{
    public class TlkrScrollViewer : ScrollViewer
    {
        // this is a property that is viewed on the XAML 
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",                             // Name of property on XAML
            typeof(string),                     // type of property on XAML
            typeof(TlkrScrollViewer),                    // who inherits this property
            new UIPropertyMetadata(null)); 


        public string Text // Not required but recommended create variable with SAME NAME as Dependecy property
        {
            get { return (string)GetValue(TextProperty); } 
            set { SetValue(TextProperty, value); }
        }
    }
}
