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

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public static readonly DependencyProperty AutoSelectedProperty = DependencyProperty.RegisterAttached("AutoSelected", typeof(bool), typeof(Item), new PropertyMetadata(false));

        public bool AutoSelected
        {
            get { return (bool)GetValue(AutoSelectedProperty); }
            set { SetValue(AutoSelectedProperty, value); }
        }

        public Item()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            label.Content = text;
        }

        public void SetImage(string path)
        {
            image.Width = 200;
            image.Height = 200;
            image.Source = new BitmapImage(new Uri(path));
        }
    }
}
