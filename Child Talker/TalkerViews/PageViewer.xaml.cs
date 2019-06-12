using Child_Talker.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Child_Talker.TalkerViews.PageViewerSecondaryWindow;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for PageViewer.xaml
    /// </summary>
    public partial class PageViewer : TalkerView
    {
        private ChildTalkerXmlWrapper XmlWrapper;
        private IChildTalkerTile backItem;
        private string ProfilePath;
        private List<IChildTalkerTile> rootChildren = new List<IChildTalkerTile>();
        public Stack<ChildTalkerFolder> ViewParents = new Stack<ChildTalkerFolder>();

        private Utilities.Autoscan scan;

        public PageViewer()
        {
            InitializeComponent();

            ProfilePath = "../../Resources/example2.xml";

            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.jpg", this);
            if (!File.Exists(ProfilePath))
            {
                File.Create(ProfilePath);
            }
            else
            {
                this.LoadFromXml(ProfilePath);
            }
            scan = Utilities.Autoscan.Instance;
        }

        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                Controls,
                items
            };
            return (parents);
        }

        /// <summary>
        /// Getter method for ViewParents
        /// </summary>
        /// <returns></returns>
        public Stack<ChildTalkerFolder> getViewParents()
        {
            return ViewParents;
        }

        public void LoadFromXml(string _path)
        {
            ProfilePath = _path;
            ViewParents = new Stack<ChildTalkerFolder>();
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlReader reader = XmlReader.Create(_path))
            {
                XmlWrapper = (ChildTalkerXmlWrapper) serializer.Deserialize(reader);
            }

            List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
            foreach (var child in XmlWrapper.Children)
            {
                ctTiles.Add(ParseNode(child));
            }

            LoadTiles(ctTiles, true);
        }

        public void SaveToXml(string _path)
        {
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlWriter writer = XmlWriter.Create(_path))
            {
                serializer.Serialize(writer, XmlWrapper);
            }
        }

        public IChildTalkerTile ParseNode(ChildTalkerXml _node)
        {
            if (_node.TileType == ChildTalkerXml.Tile.talker)
            {
                return new ChildTalkerTile(_node.Text, _node.ImagePath);
            } else
            {
                List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
                foreach (var child in _node.Children)
                {
                    ctTiles.Add(ParseNode(child));
                }
                ChildTalkerFolder folder = new ChildTalkerFolder(_node.Text, _node.ImagePath, this);
                folder.SetChildren(ctTiles);
                return folder;
            }
        }
        
        public void GoBackPressTileScanning(Autoscan.HighlightedElementInfo hei)
        {
            try {
                if (ViewParents.Count<1 && !scan.GoBackDefaultEnabled)
                {
                    // by calling this here default key behavior will happen immediately after invocation is completet
                    scan.GoBackDefaultEnabled = true;
                    scan.GoBackPress -= GoBackPressTileScanning;
                }
                backItem.PerformAction(); }
            catch { Autoscan.Instance.GoBackPress -= GoBackPressTileScanning; }
        } 

        public void LoadTiles(List<IChildTalkerTile> _ctTiles, Boolean calledFromLoad = false)
        {
            //only true when page is first opened
            if (calledFromLoad) 
            {
                rootChildren = _ctTiles;
            }
            else
            {
                // only true when opening folders
                if(!_ctTiles.Contains(backItem) && ViewParents.Count > 0)
                {
                    _ctTiles.Insert(0, backItem);
                    ViewParents.Peek().SetChildren(_ctTiles);

                    // this will ony trigger the first time LoadTiles is Called
                    if (scan.GoBackDefaultEnabled)
                    {
                        scan.GoBackDefaultEnabled = false;
                        scan.GoBackPress += GoBackPressTileScanning;
                    }
                }
            }

            items.Children.Clear();
            foreach (IChildTalkerTile item in _ctTiles)
            {
                Item ui = new Item();
                ui.SetItem(item);
                ui.SetParent(this);
                items.Children.Add(ui);
            }

            if (calledFromLoad)
            {
                XmlWrapper.Children = new List<ChildTalkerXml>();
                foreach (IChildTalkerTile rootChild in _ctTiles)
                {
                    XmlWrapper.Children.Add(rootChild.Xml);
                }
            }
        }

        public void AddSingleItem(IChildTalkerTile _ctTileToAdd)
        {
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().AddChild(_ctTileToAdd);
            }
            else
            {
                rootChildren.Add(_ctTileToAdd);
                XmlWrapper.Children.Add(_ctTileToAdd.Xml);
            }
            Item ui = new Item();
            ui.SetItem(_ctTileToAdd);
            ui.SetParent(this);
            items.Children.Add(ui);

            SaveToXml(ProfilePath);
        }

        public void RemoveSingleTile(Item _itemToRemove)
        {
            items.Children.Remove(_itemToRemove);
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().RemoveChild(_itemToRemove.CtTile);
            }
            else
            {
                rootChildren.Remove(_itemToRemove.CtTile);
                XmlWrapper.Children.Remove(_itemToRemove.CtTile.Xml);
            }

            SaveToXml(ProfilePath);
        }

        public void PopFolderView()
        {
            if (ViewParents.Count < 2)
            {
                ViewParents.Pop();
                LoadTiles(rootChildren);
            }
            else
            {
                ViewParents.Pop();
                List<IChildTalkerTile> children = ViewParents.Peek().Children;
                LoadTiles(children);
            }
        }

        private SecondaryWindow popupWindow;
        private KeyboardLayout keyboard;
        public void CreateFolder(object sender, RoutedEventArgs e)
        {
            String inputPhrase = keyboard.textBox.Text;
            if (inputPhrase != "")
            {
                ChildTalkerFolder ctFolder = new ChildTalkerFolder(inputPhrase, "../../Resources/folder.jpg", this);
                AddSingleItem(ctFolder);
            }
        }
        



        private void InsertFolderTile_Click(object sender, RoutedEventArgs e)
        {
            NewKeyboardPopup();
            keyboard.defaultEnterPress = false;
            keyboard.EnterPress += CreateFolder;
            popupWindow.Show();
        }

        private void NewKeyboardPopup()
        {
            keyboard = new KeyboardLayout();
            popupWindow = new SecondaryWindow((MainWindow)Window.GetWindow(this));
            keyboard.AddTextBox();
            keyboard.textBox.CharacterCasing = CharacterCasing.Lower;
            keyboard.textBox.MaxLength = 25;
            //redefines size of secondaryWindow to fit keyboard
            popupWindow.MainGrid.Margin = new Thickness(130, 0, 130, 0);
            popupWindow.MainGrid.DataContext = keyboard.MainGrid;
            popupWindow.SetContents<Panel>(keyboard.keyboardGrid);

            // invocations
            keyboard.BackPressWhenEmpty += ((object sender, RoutedEventArgs e) => { popupWindow.Close(); });
            if (Autoscan.flagAutoscanActive) 
            {
                scan.GoBackPress += (hei => {
                    if (hei.parentPanel != null) return;
                    popupWindow?.Close();
                    scan.IgnoreGoBackPressOnce = true;
                });
            }
        }

        private void InsertTalkerTile_Click(object sender, RoutedEventArgs e)
        {
            NewKeyboardPopup();
            keyboard.defaultEnterPress = false;
            keyboard.EnterPress += CreateTile;
            popupWindow.Show();
        }

        private void OnIconSelect(string imagePath)
        {
            if (inputPhrase != "")
            {
                popupWindow.Close();
                
                //String imagePath = PromptFileExplorer();
                if (imagePath != "")
                {
                    ChildTalkerTile ctItem = new ChildTalkerTile(inputPhrase, imagePath);
                    AddSingleItem(ctItem);
                }
            }

        }

        private string inputPhrase = "";
        private void CreateTile(object sender, RoutedEventArgs e)
        {
            inputPhrase = keyboard.textBox.Text;
            popupWindow.Close();

            popupWindow = new SecondaryWindow((MainWindow)Window.GetWindow(this));
            ImageGenerator ig = new ImageGenerator(OnIconSelect);
            popupWindow.SetContents(ig);
            popupWindow.Show();

            ig.setAutoscanFocus(popupWindow);
            ig.CancelIcon.Click += (bSender, bE) =>
            {
                scan.GoBackPressDefault();
                popupWindow?.Close();
                scan.StartAutoscan(this.getParents());
                scan.UpdateActiveWindow(MainWindow._mainWindow);
            };
/*
*/
        }

        private void GenerateImageExplorer()
        {
        }

        private String PromptFileExplorer()
        {
            Stream myStream = null;
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
            return imagePath;
        }
    }
}
