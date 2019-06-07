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
using System.Windows.Shapes;
using Child_Talker.Utilities;


namespace Child_Talker.TalkerView
{
    /// <summarys
    /// Interaction logic for WindowHistory.xaml
    /// </summary>
    public partial class SettingsPage : TalkerView
    {
        SettingsPage()
        {
            InitializeComponent();        
        }


        /* Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         * Currently goes between the phrase stack and side menu
         */
        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                phraseStack,
                sidePanel
            };
            return (parents);
        }
    }
}
