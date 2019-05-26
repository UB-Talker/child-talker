using System.Windows;
using System.Windows.Controls;

namespace Child_Talker
{
    public class TlkrGrid : Grid
    {
        // this is a property that is viewed on the XAML 
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",                             // Name of property on XAML
            typeof(string),                     // type of property on XAML
            typeof(TlkrGrid),                    // who inherits this property
            new UIPropertyMetadata(null)); 


        public string Text // Not required but recommended create variable with SAME NAME as Dependecy property
        {
            get { return (string)GetValue(TextProperty); } 
            set { SetValue(TextProperty, value); }
        }

    }
}
