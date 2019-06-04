using System;
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
        public void autoscanButton_click(object sender, RoutedEventArgs e)
        {
            Autoscan autosc = Autoscan._instance; //singleton cannot call constructor, call instance
            autosc.updateActiveWindow(this.getWindow());
            if (autosc.isScanning())
            {
                autosc.stopAutoscan();
            }
            else
            {
                autosc.startAutoscan(this.getParents()); //updates autoscan on what the current view i
            }
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


        override public List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>();
            parents.Add(row0);
            parents.Add(row1);
            parents.Add(row2);
//            parents.Add(row3);
            return (parents);
        }
    }
    
}
