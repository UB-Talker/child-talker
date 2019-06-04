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
using Microsoft.VisualBasic;
using Child_Talker.TalkerViews;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public static readonly DependencyProperty AutoSelectedProperty = DependencyProperty.RegisterAttached("AutoSelected", typeof(bool), typeof(Item), new PropertyMetadata(false));
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        public IChildTalkerTile CtTile { get; set; }
        private PageViewer Root;

        public bool AutoSelected
        {
            get { return (bool)GetValue(AutoSelectedProperty); }
            set { SetValue(AutoSelectedProperty, value); }
        }

        public Item()
        {
            InitializeComponent();
        }

        public void SetParent(PageViewer _parent)
        {
            Root = _parent;
        }

        public void SetItem(IChildTalkerTile _ctItem)
        {
            CtTile = _ctItem;

            label.Text = _ctItem.Text;
            image.Width = 200;
            image.Height = 200;
            image.Source = new BitmapImage(new Uri(_ctItem.ImagePath, UriKind.RelativeOrAbsolute));
           
        }

        public new Brush Background
        {
            get { return tkrbtn.Background; }
            set { tkrbtn.Background = value; }
        }

        public Brush TkrForeground
        {
            get { return tkrbtn.TkrForeground; }
            set { tkrbtn.TkrForeground = value; }
        }

        private void LeftMouseButton_Click(object sender, RoutedEventArgs e)
        {  
            CtTile.PerformAction();
        }

        private void RightMouseButton_Up(object sender, MouseButtonEventArgs e)
        {
            if (!(CtTile is ChildTalkerBackButton) && !(CtTile is ChildTalkerFolderAdder) && !(CtTile is ChildTalkerTileAdder))
            {
                MsgBoxResult response = Interaction.MsgBox("Would you like to delete this tile?", MsgBoxStyle.YesNo, "Delete Tile");
                if (response == MsgBoxResult.Yes)
                {
                    Root.RemoveSingleTile(this);
                }
            }
        }

        public TlkrBTN getTlkrBTN()
        {
            return tkrbtn;
        }
    }
}
