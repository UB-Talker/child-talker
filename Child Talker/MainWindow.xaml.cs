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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Stack<TalkerView> previousViews;
        private TextUtility util;
       
        Autoscan autosc;

        public MainWindow()
        {

            InitializeComponent();

            util = TextUtility.Instance;
            previousViews = new Stack<TalkerView>();

            TalkerView startScreen = new MainMenu();
            DataContext = startScreen;  //DataContext will give you the current view

            this.Closing += save;
        }


        /*
         * Change the view to the previous view. The views will be maintained in a stack of TalkerViews.
         */
        public void back()
        {

            TalkerView lastView = previousViews.Peek();
            previousViews.Pop();
            lastView.update();
            changeView(lastView);
        }

        public void toggleAutoscan(object source, RoutedEventArgs e)
        {
            if (autosc == null)
            {
                autosc = Autoscan._instance; //singleton cannot call constructor, call instance
            }
            if (autosc.isScanning())
            {
                autosc.stopAutoscan();
            }else
            {
                autosc.startAutoscan(this); //updates autoscan on what the current view is
            }
        }

        // Method to change TalkerView, primarily called by TalkerView itself
        public void changeView(TalkerView view)
        {
            setPreviousView(this.DataContext as TalkerView);
            DataContext = view;

            if(autosc != null && autosc.isScanning())
            {
                IInputElement what = System.Windows.Input.Keyboard.FocusedElement;
                // increments index for next button 
                autosc.stopAutoscan();
                autosc.startAutoscan(this);

                //view.Focusable = true;
                //view.Focus();
                IInputElement what2 = System.Windows.Input.Keyboard.FocusedElement;
                // increments index for next button
            }

        }

       

        /*
         * Sets the previous view to a reference of a TalkerView.
         * This view is pushed to the top of the previousViews Stack
         */

        public void setPreviousView(TalkerView view)
        {
            previousViews.Push(view);
        }


        /*
         * Resets the Stack of TalkerViews. Used when the user returns straight to the MainMenu
         */
        public void resetStack()
        {
            previousViews = new Stack<TalkerView>();
        }


        /*
         * Save speech history when closed
         */
        private void save(object sender, EventArgs args)
        {
            util.save();
        }
    }
}
