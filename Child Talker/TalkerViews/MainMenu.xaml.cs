using System.Collections.Generic;
using System.Windows;

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
