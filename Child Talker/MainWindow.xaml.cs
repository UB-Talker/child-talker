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

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();

            //viewer.LoadFromXml("../../Resources/example2.xml");
            //viewer.StartAutoScan();

            //uncomment to display specific window change WindowHistory to desired Window
            //WindowHistory temp = new WindowHistory();
            Keyboard temp = new Keyboard();
            //MenuPage temp = new MenuPage();
            temp.Show();
            this.Close();
        }
    }
}
