using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Child_Talker.Utilities;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for ImageGeneratorTest.xaml
    /// </summary>
    public partial class ImageGeneratorTest : SecondaryWindow
    {
        internal string path = "../../Resources/General_Icons";
        private readonly Autoscan2 scan;

        private string selectedImagePath;

        public ImageGeneratorTest()
        {
            InitializeComponent();
            scan = Autoscan2.Instance;

            ImagesPanel.ScrollOwner = scrollViewer;
            this.CancelIcon.Click += (bSender, bE) => this.Close();
            GetCurrentDirectoryContents(path);
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

            this.CancelIcon.Click += (bSender, bE) => this.Close();

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
                else (Window.GetWindow(this) as SecondaryWindow)?.Close();
            }
        }

        public void GetCurrentDirectoryContents(string newPath)
        {
            this.Dispatcher.Invoke(() =>
            {
                ImagesPanel.Children.Clear();
                path = newPath;
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                StackPanel row = new StackPanel()
                {
                    Width = 1100,
                    Orientation = Orientation.Horizontal
                };
                int columnCount = 0;

                foreach (var directory in directories)
                {
                    string[] firstFile = Directory.GetFiles(directory);
                    if (firstFile.Length <= 0) continue;
                    ImageGenButton button = new ImageGenButton(directory, firstFile[0])
                    {
                        ActionType = ImageGenButton.ActionTypeEnum.Folder,
                        BorderBrush = Brushes.Blue
                    };

                    button.PassImageOnClick += GetCurrentDirectoryContents;
                    if (columnCount < 4)
                    {
                        row.Children.Add(button);
                        columnCount++;
                    }
                    else
                    {
                        ImagesPanel.Children.Add(row);
                        columnCount = 0;
                        row = new StackPanel()
                        {
                            Width = 1100,
                            Orientation = Orientation.Horizontal
                        };
                    }

                    Console.WriteLine(directory);
                }

                foreach (var file in files)
                {
                    ImageGenButton button = new ImageGenButton(file, file)
                    {
                        ActionType = ImageGenButton.ActionTypeEnum.Folder,
                        BorderBrush = Brushes.Red
                    };
                    button.PassImageOnClick += ImageSelected;
                    if (columnCount < 4)
                    {
                        row.Children.Add(button);
                        columnCount++;
                    }
                    else
                    {
                        ImagesPanel.Children.Add(row);
                        columnCount = 0;
                        row = new StackPanel()
                        {
                            Width = 1100,
                            Orientation = Orientation.Horizontal
                        };
                    }
                    //Console.WriteLine(file);
                }

                if (columnCount != 0)
                {
                    ImagesPanel.Children.Add(row);
                }
            });
        }

        public List<DependencyObject> GetParents()
        {
            return new List<DependencyObject>() { ImagesPanel, CancelIcon };
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
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            ImageSelected(imagePath);
        }
    }
}
