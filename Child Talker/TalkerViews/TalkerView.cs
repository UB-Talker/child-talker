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
    public class TalkerView : Page
    {
        /*
         * Set the current view to the main menu and clear the Stack maintained by the
         * MainWindow. This is used for the user to jump straight back to the MainMenu
         * from wherever they are.
         */
        public void backToHome(object sender, RoutedEventArgs args)
        {
            MainWindow.Instance.ResetStack();
            MainWindow.Instance.ChangeView(new MainMenu());
        }


        /*
         * This functionality will be assigned to the back button on the pages that implement it.
         * See MainWindow.back method to see more information.
         */
        public void OpenPreviousView(object sender, RoutedEventArgs args)
        { 
            MainWindow.Instance.Back();
        }



        /// <summary>
        /// May be needed to update some views when navigating between them.
        /// This method is 'virtual' because it allows subclasses to override this definition
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// When overridden this will act as the topmost layer for autoscan to scan through
        /// </summary>
        /// <returns></returns>
        public virtual List<DependencyObject> GetParents() => null;

    }
}
