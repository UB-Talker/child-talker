using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Child_Talker.TalkerViews.PageViewerSecondaryWindow
{
    /// <summary>
    /// Interaction logic for TalkerButtonLayout.xaml
    /// </summary>
    public partial class TlkrBtnLayout : TlkrBTN
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
        public TlkrBtnLayout(string path, string imagePath)
        {
            InitializeComponent();
            ImagePath = imagePath;
            currentPath = path;
            image.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            var substring = path.Substring(path.LastIndexOf('\\')+1);
            label.Content  = substring;
        }


        private void ThisOnClick(object sender, RoutedEventArgs e)
        {
            PassImageOnClick?.Invoke(currentPath);
        }
    }
}
