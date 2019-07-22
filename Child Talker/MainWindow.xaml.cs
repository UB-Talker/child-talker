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
        public double innerHeight;
        public double innerWidth;

        private static readonly Autoscan2 scan = Autoscan2.Instance;
        private static MainWindow _instance;
        public static MainWindow Instance => _instance;

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;

            //scan = AutoscanOriginal.Instance;
                //this.Content = startScreen; //DataContext will give you the current view
            scan.NewWindow(this);
            this.Closing += (sender, e) => { TextUtility.Instance.save(); };
            this.Closing += (sender, e) => { Application.Current.Shutdown(); };
        }



        /*
         * Change the view to the previous view. The views will be maintained in a stack of TalkerViews.
         */
        public void Back()
        {
            if (Navigator.CanGoBack)
               Navigator.GoBack();
            else if (Navigator.CanGoForward)
                Navigator.GoForward();
        }

        public bool BackIsEmpty()
        {
            return Navigator.CanGoBack;
            //   return (previousViews.Count == 0);
        }

        // Method to change TalkerView, primarily called by TalkerView itself
        public void ChangeView(TalkerView view)
        {
            Navigator.Navigate(view);
        }

        private void NewPageIsLoaded(object sender, EventArgs e)
        {
            scan.ClearReturnPointList();
            Frame f = ((Frame)sender);
            if (f.Content is TalkerView tv && tv.GetParents() != null && tv.GetParents().Any())
            {
                scan.NewListToScanThough(tv.GetParents(), true);
            }
            else
            {
                scan.NewListToScanThough<Panel>(f.Content as Panel, true);
            }
        }

        public void ExecutedCustomCommand(object sender,
            ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Custom Command Executed");
        }
        public void CanExecuteCustomCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            Control target = e.Source as Control;

            if (target != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// When the Navigat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Navigator_OnLoaded(object sender, RoutedEventArgs e)
        {
            Frame navi = (Frame)sender;
            scan.NewListToScanThough(((TalkerView)navi.Content).GetParents());
        }
    }
}
