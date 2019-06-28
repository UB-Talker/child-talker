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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImageGenerator : SecondaryWindow
    {
        internal string path = "../../Resources/General_Icons";
        public ImageGenButton.OnClickPath onImageSelectHandler; //input parameter of type string
        public event ImageGenButton.OnClickPath OnImageSelect;

        private Autoscan2 scan;
        public ImageGenerator(ImageGenButton.OnClickPath onImageSelect)
        {
            InitializeComponent();
            this.OnImageSelect = onImageSelect;
            scan = Autoscan2.Instance;
            ImagesPanel.ScrollOwner = scrollViewer;
            GetCurrentDirectoryContents(path);
        //    scan.StartAutoscan<Panel>(ImagesPanel);

        }

        public new void Show()
        {
            scan.GoBackDefaultEnabled = false;
            scan.GoBackPress += (hei) =>
                this.GoBackIcon?.RaiseEvent(
                    new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)
                    ); // how you simulate a button click in code

            this.CancelIcon.Click += (bSender, bE) =>
            {
                this.Close();
            };

            this.setAutoscanFocus(this);
            this.Show<Panel>(ImagesPanel);

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
                    button.PassImageOnClick += OnImageSelect;
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
        /// <summary>
        /// what this class needs to set up on a new window
        /// </summary>
        /// <param name="newFocus"></param>
        public void setAutoscanFocus(Window newFocus)
        {
            scan.SelectPress += SelectPress;
        }

        private void SelectPress(DependencyObject currentObj)
        {
            if (currentObj is ImageGenButton button)
            {
                button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
                scan.IgnoreSelectPressOnce=true;
                scan.NewListToScanThough<Panel>(ImagesPanel);
            }
            else
            {
                if (currentObj is StackPanel)
                {
                    inMainGrid = false;
                }

            }
        }
        
        public List<DependencyObject> GetParents()
        {
            return new List<DependencyObject>(){ImagesPanel, CancelIcon};
        }

        private bool inMainGrid = false;
        private void GoBackPage(object sender, RoutedEventArgs e)
        {
            if(path.Contains("\\"))
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
                } else (Window.GetWindow(this) as SecondaryWindow)?.Close();
            }
        }

        private void PromptFileExplorer(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;

            String imagePath = "";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    imagePath = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            OnImageSelect?.Invoke(imagePath);
        }

    }
}
