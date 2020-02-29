using System;
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;
using Brushes = System.Drawing.Brushes;
using Button = Child_Talker.TalkerButton.Button;
using Image = System.Drawing.Image;
using Timer = System.Timers.Timer;
using MessageBox = Child_Talker.Utilities.MessageBox;
using MessageBoxResult =Child_Talker.Utilities.MessageBoxResult;
using MessageBoxButton =Child_Talker.Utilities.MessageBoxButton;
using Size = System.Windows.Size;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class PhraseButton : Button
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        public IChildTalkerTile CtTile { get; set; }
        private Phrases Root;
        public PhraseButton()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { Text = Text; };
        }

        private new string Text
        {
            get => (this as Button).Text;
            set
            {
                (this as Button).Text = value;
                if(IsLoaded)
                {
                    Size textsize = MeasureString(value);
                    var fullSize = this.Width + this.Margin.Left + this.Margin.Right;
                    var textWidth = textsize.Width;
                    var txtblockwidth = 300.0;
                    while (textWidth>= txtblockwidth-60)
                    {
                        this.Dispatcher.Invoke( () =>
                        {
                            this.Width += fullSize;
                        });
                        txtblockwidth += fullSize;
                    }

                }
            }
        }
        public void SetParent(Phrases _parent)
        {
            Root = _parent;
        }

        public void SetItem(IChildTalkerTile _ctItem)
        {
            CtTile = _ctItem;
            //this.Text = _ctItem.Text;

            this.Width = 265;
            this.Height = 265;

            this.Text = _ctItem.Text;
            this.ImageSource = _ctItem.ImagePath;
            this.InColor = _ctItem.InColor;
                //new BitmapImage(new Uri(_ctItem.ImagePath, UriKind.RelativeOrAbsolute));
        }
        
        
        /// <summary>
        /// checks to see if a given image has transparency
        /// </summary>
        /// <param name="ImagePath">FilePath to Image</param>
        /// <returns>true if Image is Transparent. False if path is invalie</returns>
        public static bool CheckForTransparency(string ImagePath)
        {
            Image img;
            try
            {
                img = Image.FromFile(ImagePath, true); 
            }
            catch { return false; }


            if ((img.Flags & 0x2) != 0)
            {
                Bitmap image = new Bitmap(img);
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image.GetPixel(x, y).A != 255)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void LeftMouseButton_Click(object sender, RoutedEventArgs e)
        {  
            CtTile.PerformAction();
        }
        
        private void RightMouseButton_Up(object sender, MouseButtonEventArgs e)
        {
            ModifyThis();
        }
        
        ///Deleted variable is lazy way around repetition issue when attempting to delete element. creates delay after window is opened
        /// see <see cref="ModifyThis"/> for usage
        private bool Deleted = false;
        /// <summary>
        /// A popup appears asking if The user would like to delete this element
        /// </summary>
        /// <returns> True if the deletion occurred</returns>
        public bool ModifyThis()
        {
            if (Deleted) return false;
            if (this.CtTile is ChildTalkerBackButton) return false;

            Deleted = true;
            //MsgBoxResult response = Interaction.MsgBox("Would you like to delete this tile?", MsgBoxStyle.YesNo, "Delete Tile");
            MessageBoxResult response = MessageBox.ShowYesModifyCancel("Would you like to delete The following phrase? \n->\"" + this.Text+"\"","Yes", "Modify", "Cancel");
            switch (response)
            {
                case MessageBoxResult.Yes:
                    Root.RemoveSingleTile(this);
                    return true;
                case MessageBoxResult.Modify:
                    var newPath = ImageGenerator.ImagePopup();
                    if (string.IsNullOrEmpty(newPath)) break;
                    ImageSource = newPath;
                    this.IsSelected = false;
                    Root.UpdateSavedTiles(this, ImageSource);
                    return true;
            }

            Timer t = new Timer(1000);
            t.Elapsed += (st, et) => Deleted = false;
            t.Start();
            return false;
        }
    }
}
