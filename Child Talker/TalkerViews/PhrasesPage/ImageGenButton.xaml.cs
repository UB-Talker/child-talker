using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Child_Talker.TalkerButton;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for TalkerButtonLayout.xaml
    /// </summary>
    public partial class ImageGenButton : Button
    {
        public string ImagePath;


        public enum ActionTypeEnum 
        {
            Icon,
            Folder
        };

        public delegate void OnClickPath(string s);

        public event OnClickPath PassImageOnClick;

        public ActionTypeEnum ActionType;
        public string currentPath;
        public ImageGenButton(string path, string imagePath)
        {
            InitializeComponent();
            ImagePath = imagePath;
            currentPath = path;

            var substring = path.Substring(path.LastIndexOf('\\')+1);
            this.Text = substring;
            this.ImageSource = imagePath;
        }


        private void ThisOnClick(object sender, RoutedEventArgs e)
        {
            PassImageOnClick?.Invoke(currentPath);
        }
    }
}
