using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Child_Talker.TalkerViews
{
    public class TalkerView : UserControl
    {
        


        public TalkerView(){}

        /*
         * Helper method that returns a reference to the current MainWindow
         */
        private MainWindow getWindow()
        {
            return (MainWindow)Window.GetWindow(this);
        }


        /*
         * Set the current view to the main menu and clear the Stack maintained by the
         * MainWindow. This is used for the user to jump straight back to the MainMenu
         * from wherever they are.
         */
        public void backToHome(object sender, RoutedEventArgs args)
        {
            MainWindow window = getWindow();
            window.resetStack();
            window.DataContext = new MainMenu();
        }



        /*
         * Set the current view to the keyboard.
         */
        public void openKeyboard(object sender, RoutedEventArgs args)
        {
            MainWindow window = getWindow();
            window.setPreviousView(this);

            window.DataContext = new UserControl1();
        }


        /*
         * Set the current view to the history view.
         */
        public void openHistory(object sender, RoutedEventArgs args)
        {
            MainWindow window = getWindow();
            window.setPreviousView(this);

            window.DataContext = new WindowHistory();
        }


        /*
         * Sets the current view to the Phrases view.
         */
        public void openPhrases(object sender, RoutedEventArgs args)
        {
            MainWindow window = getWindow();
            window.setPreviousView(this);

            window.DataContext = new PageViewer();
        }



        /*
         * This functionality will be assigned to the back button on the pages that implement it.
         */
        public void openPreviousView(object sender, RoutedEventArgs args)
        { 
            getWindow().back();
        }


    }
}
