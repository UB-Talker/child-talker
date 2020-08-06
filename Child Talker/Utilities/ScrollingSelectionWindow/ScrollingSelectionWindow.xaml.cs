using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Child_Talker.Utilities
{
    /// <summary>
    /// Interaction logic for ScrollingSelectionWindow.xaml
    /// </summary>
    public partial class ScrollingSelectionWindow : SecondaryWindow
    {
        public string result = null;
        public ScrollingSelectionWindow()
        {
            InitializeComponent();
        }

        public void addElement(UIElement obj)
        {
            _ = List.Children.Add(obj);
        }

        public UIElementCollection GetAllElements()
        {
            return List.Children;
        }

        public string prompt()
        {
            this.Show();
            return result;
        }

    }
}
