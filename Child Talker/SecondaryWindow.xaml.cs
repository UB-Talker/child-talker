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
    /// Interaction logic for SecondaryWindow.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {
        private readonly Autoscan scan = Autoscan.Instance;
        public Window parentWindow;


        public SecondaryWindow(Window parent)
        {
            InitializeComponent();
            GeneralConstructor(parent);
        }



        public SecondaryWindow(Window parent, UserControl dataContent)
        {
            InitializeComponent();
            GeneralConstructor(parent);
            SetContents(dataContent);
        }

        public SecondaryWindow(Window parent, Panel dataContent)
        {
            InitializeComponent();
            GeneralConstructor(parent);
            SetContents(dataContent);
        }
        /// <summary>
        /// this is used by all constructors
        /// </summary>
        private void GeneralConstructor(Window parent)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            parentWindow = parent;
            this.Closed += CloseWindow; //scan.GoBackCloseSecondaryWindow triggered here
            scan.ClearParentPanel();
        }

        /// <summary>
        /// Sets Contents of SecondaryWindow and loads up autoscan
        /// used to scan through entire UserControl
        /// will try TalkerView.getParents if this fails will get all Buttons from page
        /// </summary>
        /// <param name="view"></param>
        public void SetContents(UserControl view)
        {
            DataContext = view;
            try
            {
                scan.StartAutoscan((view as TalkerView).getParents());
            }
            catch
            {
                scan.StartAutoscan(Autoscan.GenerateObjectList<Button>(view, new List<DependencyObject>() ));
            }
        }
        /// <summary>
        /// Sets Contents of SecondaryWindow and loads up autoscan
        /// scans through all buttons of provided panel
        /// </summary>
        /// <param name="view"></param>
        public void SetContents(Panel view)
        {
            DataContext = view;
            scan.StartAutoscan<Button>(view, this);
        }
        /// <summary>
        /// MUST BE APPLIED MANUALLY
        /// Sets Contents of SecondaryWindow and loads up autoscan
        /// scans through all elements of type T in provided panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        public void SetContents<T>(Panel view) where T : DependencyObject
        {
            DataContext = view;
            scan.StartAutoscan<T>(view, this);
        }


        /// <summary>
        /// Default Behavior For Closing SecondaryDisplay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, EventArgs e)
        {
            scan.GoBackDefaultEnabled = true;
            scan.GoBackCloseSecondaryWindow(this, e);
        }
    }
}
