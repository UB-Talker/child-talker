using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for PageViewer.xaml
    /// </summary>
    public partial class PageViewer : UserControl
    {
        private Timer timer;
        private int i = 0;

        public PageViewer()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
        }

        public void StartAutoScan()
        {
            i = 0;
            ((Item)this.items.Children[i]).AutoSelected = true;
            timer.Start();
        }

        public void StopAutoScan()
        {
            ((Item)this.items.Children[i]).AutoSelected = false;
            timer.Stop();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Console.WriteLine($"{i} / {items.Children.Count}");
                ((Item)this.items.Children[i]).AutoSelected = false;
                i = (i + 1) % this.items.Children.Count;
                ((Item)this.items.Children[i]).AutoSelected = true;
            });
        }

        public void SetItems(List<ChildTalkerItem> items)
        {
            this.items.Children.Clear();
            foreach (ChildTalkerItem item in items)
            {
                Item ui = new Item();
                ui.SetText(item.Text);
                ui.SetImage(item.ImagePath);
                this.items.Children.Add(ui);
            }
        }
    }
}
