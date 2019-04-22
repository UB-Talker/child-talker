using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : TalkerView
    {
        public MainMenu()
        {

            InitializeComponent();
        }

        /* Used for autoscan, please update if xaml is changed
        * Must return the panels to iterate through when autoscan is first initialized on this page
        */
        override public List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>();
            parents.Add(row0);
            parents.Add(row1);
            parents.Add(row2);
            parents.Add(row3);
            return (parents);
        }
    }
    
}
