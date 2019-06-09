using System.Collections.Generic;
using System.Windows;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : TalkerView
    {
        private Utilities.Autoscan scan;
        public MainMenu()
        {
            InitializeComponent();
        }

        /* Used for autoscan, please update if xaml is changed
        * Must return the panels to iterate through when autoscan is first initialized on this page
        */
        public void autoscanButton_click(object sender, RoutedEventArgs e)
        {
            scan = Utilities.Autoscan.instance; //singleton cannot call constructor, call instance
            scan.updateActiveWindow(this.getWindow());
            if (scan.isScanning())
            {
                scan.stopAutoscan();
            }
            else
            {
                scan.StartAutoscan(this.getParents()); //updates autoscan on what the current view i
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


        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                row0,
                row1,
                row2
            };
//            parents.Add(row3);
            return (parents);
        }
    }
    
}
