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
        public MainWindow getWindow()
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
            getWindow().changeView(new MainMenu());
          
        }

        /*
         * Set the current view to the keyboard.
         */
        public void openKeyboard(object sender, RoutedEventArgs args)
        {
            MainWindow window = getWindow();
            getWindow().changeView(new Keyboard());

        }


        /*
         * Set the current view to the history view.
         */
        public void openHistory(object sender, RoutedEventArgs args)
        {
            WindowHistory newHist = new WindowHistory();
            getWindow().changeView(newHist);
        }

       

        /*
         * Sets the current view to the Phrases view.
         */
        public void openPhrases(object sender, RoutedEventArgs args)
        {
            PageViewer pv = new PageViewer();
            getWindow().changeView(pv);
            
        }

        public void openEnvControls(object sender, RoutedEventArgs args)
        {
            getWindow().changeView(new EnvControls());
        }

        /*
         * This functionality will be assigned to the back button on the pages that implement it.
         * See MainWindow.back method to see more information.
         */
        public void openPreviousView(object sender, RoutedEventArgs args)
        { 
            getWindow().back();
        }

        public void autoscanButton_click(object sender, RoutedEventArgs e)
        {
            getWindow().toggleAutoscan(); 
        }


        /* May be needed to update some views when navigating between them.
         * This method is 'virtual' because it allows subclasses to override
         * this definition
         */
        virtual public void update() { }

        virtual public List<DependencyObject> getParents() { return (null); }
    }
}
