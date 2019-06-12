using System;
using System.Collections.Generic;
using System.IO;
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
using Child_Talker.Utilities;
using Path = System.IO.Path;

namespace Child_Talker.TalkerViews.PageViewerSecondaryWindow
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImageGenerator : TalkerView
    {
        internal string path = "../../Resources/General_Icons";
        public TlkrBtnLayout.OnClickPath onImageSelectHandler; //input parameter of type string
        public event TlkrBtnLayout.OnClickPath OnImageSelect;

        private Autoscan scan;
        public ImageGenerator(TlkrBtnLayout.OnClickPath onImageSelect)
        {
            InitializeComponent();
            this.OnImageSelect = onImageSelect;
            scan = Autoscan.Instance;
            ImagesPanel.ScrollOwner = scrollViewer;
            GetCurrentDirectoryContents(path);
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
                    TlkrBtnLayout button = new TlkrBtnLayout(directory, firstFile[0])
                    {
                        ActionType = TlkrBtnLayout.ActionTypeEnum.Folder,
                        BorderBrush = Brushes.Yellow
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
                    TlkrBtnLayout button = new TlkrBtnLayout(file, file)
                    {
                        ActionType = TlkrBtnLayout.ActionTypeEnum.Folder,
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

        public void setAutoscanFocus(Window newFocus)
        {
            scan.UpdateActiveWindow(newFocus);
            scan.SelectPress += SelectPress;
            scan.StartAutoscan<Button>(ImagesPanel);
        }

        public void SelectPress(Autoscan.HighlightedElementInfo hei)
        {
            if (hei.highlightedObject is TlkrBtnLayout button)
            {
                button?.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); // how you simulate a button click in code
                scan.IgnoreSelectPressOnce=true;
                scan.StartAutoscan<Panel>(ImagesPanel);
            }
        }
        

        public override List<DependencyObject> getParents()
        {
            return new List<DependencyObject>(){ImagesPanel, CancelIcon};
        }

        private void GoBackPage(object sender, RoutedEventArgs e)
        {
            path = path.Substring(0, path.LastIndexOf('\\'));
            //path = path.Substring(0, path.LastIndexOf('\\'));
            if (!path.Contains("../../Resources/General_Icons"))
            {
                this.getWindow()?.Close();
            }
            else
            {
                GetCurrentDirectoryContents(path);
            }
        }

    }
}
