using Child_Talker.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

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
            scan = Utilities.Autoscan.instance;
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



        /*
         * Getter Method for ViewParents
         */
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
        
        public void GoBackPress(Autoscan.HighlightedElementInfo hei)
        {
            try {
                if (ViewParents.Count<1 && !scan.GoBackDefaultEnabled)
                {
                    scan.GoBackDefaultEnabled = true;
                    scan.GoBackPress -= GoBackPress;
                }
                backItem.PerformAction(); }
            catch { Autoscan.instance.GoBackPress -= GoBackPress; }
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
                    // scan.GoBackHold += backItem.PerformAction();
                    if (scan.GoBackDefaultEnabled)
                    {
                        scan.GoBackDefaultEnabled = false;
                        scan.GoBackPress += GoBackPress;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

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

        private void InsertFolderTile_Click(Object sender, RoutedEventArgs e)
        {
            String inputPhrase = Interaction.InputBox("Enter name for the folder you would like to add", "Enter Phrase", "Hello World!");
            if (inputPhrase != "")
            {
                ChildTalkerFolder ctFolder = new ChildTalkerFolder(inputPhrase, "../../Resources/folder.jpg", this);
                AddSingleItem(ctFolder);
            }
        }

        private void InsertTalkerTile_Click(Object sender, RoutedEventArgs e)
        {
            String inputPhrase = Interaction.InputBox("Enter the phrase you would like to add", "Enter Phrase", "Hello World!");
            if (inputPhrase != "")
            {
                String imagePath = PromptFileExplorer();
                if (imagePath != "")
                {
                    ChildTalkerTile ctItem = new ChildTalkerTile(inputPhrase, imagePath);
                    AddSingleItem(ctItem);
                }
            }
        }

        private String PromptFileExplorer()
        {
            Stream myStream = null;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
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
