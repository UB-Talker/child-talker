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

            List<ChildTalkerItem> items = new List<ChildTalkerItem>();
            items.Add(new ChildTalkerItem("Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Also Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("This might be Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("I hope his image isn't copyrighted", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Mr. Dr. Professor. Kris Schindler MD.", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Need more Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Can't stop the Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Just adding more Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Testing with Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("AHHHHHHHHH IT'S KRIS", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("add $Kris, $Kris, $Kris", @"C:\Users\Jeremy\Pictures\kris.jpg"));
            items.Add(new ChildTalkerItem("Is Kris here yet?", @"C:\Users\Jeremy\Pictures\kris.jpg"));

            viewer.SetItems(items);
            viewer.StartAutoScan();
        }
    }
}
