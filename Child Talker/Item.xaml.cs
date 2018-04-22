using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        private IChildTalkerTile item;
        private PageViewer parent;

        public bool AutoSelected
        {
            get { return (bool)GetValue(AutoSelectedProperty); }
            set { SetValue(AutoSelectedProperty, value); }
        }

        public Item()
        {
            InitializeComponent();
        }

        public void SetParent(PageViewer parent)
        {
            this.parent = parent;
        }

        public void SetItem(IChildTalkerTile item)
        {
            this.item = item;

            label.Content = item.Text;
            image.Width = 200;
            image.Height = 200;
            image.Source = new BitmapImage(new Uri(item.ImagePath, item.UriKind));
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {  
            item.PerformAction();
        }
    }
}
