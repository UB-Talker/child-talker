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
            this.Style = Application.Current.Resources["PopupStyle"] as Style;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closed += CloseWindow; //scan.GoBackCloseSecondaryWindow triggered here
        }

        public SecondaryWindow(UIElement content)
        {
            this.Style = Application.Current.Resources["PopupStyle"] as Style;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closed += CloseWindow; //scan.GoBackCloseSecondaryWindow triggered here
            Content = content;
        }

        public new void Show()
        {
            ScanThisOnClose = scan.ReturnPointList.First();
            scan.ClearReturnPointList();

            scan.NewActiveWindow(this);
            scan.NewListToScanThough<Panel>(this.Content as Panel);

            ((Window)this).Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panel"></param>
        public void Show<T>(Panel panel) where T : DependencyObject
        {
            ScanThisOnClose = scan.ReturnPointList.First();
            scan.ClearReturnPointList();
            scan.NewActiveWindow(this);
            scan.NewListToScanThough<T>(panel);

            ((Window)this).Show();
        }

        public void Show<T>() where T : DependencyObject
        {
            ScanThisOnClose = scan.ReturnPointList.First();
            scan.ClearReturnPointList();
            scan.NewActiveWindow(this);
            scan.NewListToScanThough<T>(Content as Panel);

            ((Window)this).Show();
        }

        public void Show(List<DependencyObject> scanList)
        {
            ScanThisOnClose = scan.ReturnPointList.First();
            scan.ClearReturnPointList();
            scan.NewActiveWindow(this);
            scan.NewListToScanThough(scanList);

            ((Window)this).Show();
        }

        /// <summary>
        /// Default Behavior For Closing SecondaryDisplay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender=null, EventArgs e=null)
        {
            Closed -= CloseWindow;
            scan.CloseActiveWindow();
            scan.ClearReturnPointList();
            scan.NewListToScanThough(scanThisOnClose, true);
        }
    }
}
