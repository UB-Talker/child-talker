using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Child_Talker.TalkerViews;
using Child_Talker.Utilities;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for SecondaryWindow.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        Autoscan autosc = Autoscan._instance;
        Window parentWindow;
        

        public SecondaryWindow(Window parent)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            parentWindow = parent;
        }

        // Method to change TalkerView, primarily called by TalkerView itself
        public void setContents(TalkerView view)
        {
            DataContext = view;

            if (autosc != null && autosc.isScanning())
            {
                autosc.startAutoscan(view.getParents());
            }

        }

    }
}
