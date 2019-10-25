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
    public partial class SecondaryWindow : Window
    {
        public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register(
            "InnerMargin", typeof(Thickness), typeof(SecondaryWindow), new UIPropertyMetadata(new Thickness(300,150,300,150)));
        public Thickness InnerMargin {
            get => (Thickness)GetValue(InnerMarginProperty);
            set => SetValue(InnerMarginProperty, value); }
    }
    /// <summary>
    /// Interaction logic for SecondaryWindow.xaml
    /// </summary>
    public partial class SecondaryWindow : Window
    {

        private readonly Autoscan2 scan = Autoscan2.Instance;
        private List<DependencyObject> scanThisOnClose;
        private List<DependencyObject> ScanThisOnClose
        {
            get => scanThisOnClose;
            set => scanThisOnClose = scanThisOnClose ?? value;
        }

        public SecondaryWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            scanThisOnClose = scan.ReturnPointList.Count != 0 ? scan.ReturnPointList.First() : null; //when Secondary window is closed, scan through the parentmost list from originwindow
            scan.NewWindow(this);
            this.Closed += CloseWindow; //scan.GoBackCloseSecondaryWindow triggered here
        }

        public SecondaryWindow(UIElement content)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            scanThisOnClose = scan.ReturnPointList.Count != 0 ? scan.ReturnPointList.First() : null;
            scan.NewWindow(this);
            this.Closed += CloseWindow; //scan.GoBackCloseSecondaryWindow triggered here
            Content = content;
        }

        public new void Show()
        {
            scan.ClearReturnPointList();
            //scan.NewWindow(this);
            scan.NewListToScanThough<Panel>(this.Content as Panel);

            ((Window)this).ShowDialog();
        }

        /// <summary>
        /// display a window that is hidden after initialization
        /// </summary>
        /// <typeparam name="T">Type in panel to scan for</typeparam>
        /// <param name="panel">The parent for autoscan to scan through</param>
        public void Show<T>(Panel panel) where T : DependencyObject
        {
            scan.ClearReturnPointList();
            scan.NewListToScanThough<T>(panel, true);
            ((Window)this).ShowDialog();
        }

        public void Show<T>() where T : DependencyObject
        {
            scan.ClearReturnPointList();
            scan.NewListToScanThough<T>(Content as Panel, true);
            ((Window)this).ShowDialog();
        }

        public void Show(List<DependencyObject> scanList)
        {
            scan.ClearReturnPointList();
            scan.NewListToScanThough(scanList, true);
            ((Window)this).ShowDialog();
        }


        /// <summary>
        /// Default Behavior For Closing SecondaryDisplay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender=null, EventArgs e=null)
        {
            scan.CloseActiveWindow(this);
            scan.ClearReturnPointList();
            scan.NewListToScanThough(scanThisOnClose, true);
        }
    }
}
