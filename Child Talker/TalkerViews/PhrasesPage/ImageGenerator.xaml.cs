using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Child_Talker.Utilities;
using Child_Talker.Utilities.Autoscan;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for ImageGenerator.xaml
    /// </summary>
    public partial class ImageGenerator : SecondaryWindow
    {
        public static string ImagePopup()
        { 
            ImageGenerator ig = new ImageGenerator();
            return ig.ShowImages();
        }

        internal string path = "../../Resources/General_Icons";
        private readonly Autoscan2 scan;

        private string selectedImagePath;


        public ImageGenerator()
        {
            InitializeComponent();
            scan = Autoscan2.Instance;
            scan.ResetSelectEventHandlers();
            
            ImagesPanel.ScrollOwner = scrollViewer;
            ImagesPanel.Loaded += (s,e) =>
            {
                GetCurrentDirectoryContents(path);
            };
            this.CancelIcon.Click += (bSender, bE) => this.Close();
        }
        


        /// <summary>
        /// Called to bring up window and provide user with images.
        /// Will return the path to the image that the user selected.
        /// </summary>
        /// <returns></returns>
        public string ShowImages()
        {
            scan.GoBackDefaultEnabled = false;
            scan.GoBackPress += (hei, gbp) =>
                this.GoBackIcon.RaiseEvent(
                    new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
            this.setAutoscanFocus(this);
            this.Show<Panel>(ImagesPanel);
            

            return selectedImagePath;
        }

        /// <summary>
        /// Attach this method to an image button as the event handler.
        /// Will grab the path for the image and store it in this class
        /// then close the window.
        /// </summary>
        private void ImageSelected(string path)
        {
            selectedImagePath = path;

            if (selectedImagePath != "")
            {
                Close();
            }
        }

        /// <summary>
        /// what this class needs to set up on a new window
        /// </summary>
        /// <param name="newFocus"></param>
        public void setAutoscanFocus(Window newFocus)
        {
            scan.SelectPress += SelectPress;
        }
        
        /// <summary>
        /// what to do when the Autoscan.SelectEvent Occurs
        /// </summary>
        /// <param name="currentObj"></param>
        /// <param name="selectEvent"></param>
        private void SelectPress(DependencyObject currentObj, Autoscan2.DefaultEvents selectEvent)
        {
            switch (currentObj)
            {
                case ImageGenButton button:
                    button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
                    scan.IgnoreSelectPressOnce = true;
                    scan.NewListToScanThough<Panel>(ImagesPanel);
                    break;
                case StackPanel stack:
                    inMainGrid = false;
                    break;
            }
        }

        private bool inMainGrid = false;
        private void GoBackPage(object sender, RoutedEventArgs e)
        {
            if (path.Contains("\\"))
            {
                path = path.Substring(0, path.LastIndexOf('\\'));
                GetCurrentDirectoryContents(path);
                inMainGrid = false;
            }
            else
            {
                if (!inMainGrid)
                {
                    scan.NewListToScanThough(this.GetParents());
                    inMainGrid = true;
                }
                else
                {
                    scan.IgnoreGoBackPressOnce=true;
                    Close();
                }
            }
        }

        public void GetCurrentDirectoryContents(string newPath)
        {
            this.Dispatcher.Invoke(() =>
            {
                ImagesPanel.Children.Clear();
                path = newPath;
                string[] directories_ = Directory.GetDirectories(path);
                string[] files_ = Directory.GetFiles(path);
                
                List<string> directories = directories_.ToList();
                List<string> files = files_.ToList();
                var both = directories.Concat(files);
                StackPanel row = new StackPanel()
                {
                    Width = ImagesPanel.ActualWidth,
                    Orientation = Orientation.Horizontal
                };
                double buttonsPerRow = 0;
                foreach (string current in both)
                {
                    // get the file attributes for file or directory
                    FileAttributes curAttr = File.GetAttributes(current);

                    ImageGenButton button;
                    if (curAttr.HasFlag(FileAttributes.Directory))
                    { 
                        DirectoryInfo di = new DirectoryInfo(current);
                        var imagePath = di.EnumerateFiles("*",SearchOption.AllDirectories).Select(f => f.FullName).FirstOrDefault();
                        if (string.IsNullOrEmpty(imagePath)) continue;
                        button = new ImageGenButton(current, imagePath) {
                            ActionType = ImageGenButton.ActionTypeEnum.Folder,
                            BorderBrush = Brushes.Blue  };
                        button.PassImageOnClick += GetCurrentDirectoryContents;
                        button.PassImageOnClick += (s) => { scan.rescan<Panel>(); };

                    }
                    else
                    {
                        button = new ImageGenButton(current, current) {
                            ActionType = ImageGenButton.ActionTypeEnum.Folder,
                            BorderBrush = Brushes.Red };
                        button.PassImageOnClick += ImageSelected;
                    }
                    
                    if (buttonsPerRow < row.Width-button.Width)
                    {
                        _ = row.Children.Add(button);
                        buttonsPerRow+=button.Width;
                    }
                    else
                    {
                        _ = ImagesPanel.Children.Add(row);
                        buttonsPerRow = 0;
                        row = new StackPanel()
                        {
                            Width = ImagesPanel.ActualWidth,
                            Orientation = Orientation.Horizontal
                        };
                    }

                }

            });
            scan.NewListToScanThough<Panel>(ImagesPanel);
            

        }

        public List<DependencyObject> GetParents()
        {
            return new List<DependencyObject>() { ImagesPanel, NavPanel };
        }

        private void PromptFileExplorer(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            String imagePath = "";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    imagePath = ofd.FileName;
                }
                catch (Exception ex)
                {
                    _ = System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            ImageSelected(imagePath);
        }

        private void CancelIcon_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
