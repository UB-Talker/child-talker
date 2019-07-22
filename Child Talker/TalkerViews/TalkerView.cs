using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Child_Talker.TalkerViews
{
    public abstract class TalkerView : Page
    {
        /// <summary>
        /// Navigates Back To The HomePage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void backToHome(object sender = null, RoutedEventArgs args = null)
        {
            MainWindow.Instance.Navigator.Navigate(new HomePage());
        }

        /// <summary>
        /// This is the functionality for the window to Back to the previous Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OpenPreviousView(object sender=null, RoutedEventArgs args=null)
        { 
            MainWindow.Instance.Back();
        }

        /// <summary>
        /// When overridden this will act as the topmost layer for autoscan to scan through
        /// </summary>
        /// <returns></returns>
        public abstract List<DependencyObject> GetParents();

    }
}
