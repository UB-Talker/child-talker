using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Child_Talker.TalkerViews.EnvControlsPage;
using Child_Talker.TalkerViews.PhrasesPage;
using Child_Talker.Utilities;
using EnvControls = Child_Talker.TalkerViews.EnvControlsPage.EnvControls;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : TalkerView
    {
        private readonly Utilities.Autoscan2 scan = Utilities.Autoscan2.Instance; //singleton cannot call constructor, call instance
        private readonly MainWindow mainWindow;
        public MainMenu()
        {
            InitializeComponent();
            mainWindow = MainWindow.Instance;
        }

        /// <summary>
        /// Toggles autoscan on and off
        /// </summary>
        public void autoscanButton_click(object sender, RoutedEventArgs e)
        {
            scan.ToggleAutoscan();
            scan.NewActiveWindow(MainWindow.Instance);
            scan.NewListToScanThough<Panel>(GridLayout);
        }

        /// <summary>
        /// Set the current view to the keyboard.
        /// </summary>
        public void OpenKeyboard(object sender, RoutedEventArgs args)
        {
            mainWindow.ChangeView(new Keyboard());
        }

        /// <summary>
        /// Set the current view to the history view.
        /// </summary>
        public void OpenHistory(object sender, RoutedEventArgs args)
        {
            WindowHistory newHist = new WindowHistory();
            mainWindow.ChangeView(newHist);
        }

        /// <summary>
        /// Sets the current view to the Phrases view.
        /// </summary>
        public void OpenPhrases(object sender, RoutedEventArgs args)
        {
            Phrases pv = new Phrases();
            mainWindow.ChangeView(pv);
            
        }

        /// <summary>
        /// Sets the current view to the Environmental Controls view.
        /// </summary>
        public void OpenEnvControls(object sender, RoutedEventArgs args)
        {
            mainWindow.ChangeView(new EnvControls());
        }

        public override List<DependencyObject> GetParents()
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
