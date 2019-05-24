using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using Child_Talker;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for PageViewer.xaml
    /// </summary>
    public partial class PageViewer : TalkerView
    {
        private Timer timer;
        private int i = 0;
        private bool scanning = false;
        private ChildTalkerXmlWrapper XmlWrapper;
        private IChildTalkerTile backItem;
        private string ProfilePath;
        private List<IChildTalkerTile> rootChildren = new List<IChildTalkerTile>();
        public Stack<ChildTalkerFolder> ViewParents = new Stack<ChildTalkerFolder>();

        public PageViewer()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            ProfilePath = "../../Resources/example2.xml";
            scanning = false;

            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.jpg", this);
            if (!File.Exists(ProfilePath))
            {
                File.Create(ProfilePath);
            } 
            this.LoadFromXml(ProfilePath);
        }


        /*
         * Getter Method for ViewParents
         */
        public Stack<ChildTalkerFolder> getViewParents()
        {
            return ViewParents;
        }

        public void StartAutoScan()
        {
            scanning = true;
            i = 0;
            ((Item)items.Children[i]).AutoSelected = true;
            timer.Start();
        }

        public void StopAutoScan()
        {
            scanning = false;
            if (items.Children.Count != 0)
            {
                ((Item)items.Children[i]).AutoSelected = false;
            }

            timer.Stop();
        }

        private void OnElapsedTime(object _source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Console.WriteLine($"{i} / {items.Children.Count}");
                ((Item)items.Children[i]).AutoSelected = false;
                i = (i + 1) % items.Children.Count;
                ((Item)items.Children[i]).AutoSelected = true;
            });
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

        public void LoadTiles(List<IChildTalkerTile> _ctTiles, Boolean calledFromLoad = false)
        {
            bool wasScanning = scanning;
            if (wasScanning)
            {
                StopAutoScan();
            }

            if (calledFromLoad)
            {
                rootChildren = _ctTiles;
            }
            else
            {
                if(!_ctTiles.Contains(backItem) && ViewParents.Count > 0)
                {
                    _ctTiles.Insert(0, backItem);
                    ViewParents.Peek().SetChildren(_ctTiles);
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

            if (wasScanning)
            {
                StartAutoScan();
            }
        }

        public void AddSingleItem(IChildTalkerTile _ctTileToAdd)
        {
            bool wasScanning = scanning;
            if (wasScanning)
            {
                StopAutoScan();
            }

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
            if (wasScanning)
            {
                StartAutoScan();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        public void RemoveSingleTile(Item _itemToRemove)
        {
            bool wasScanning = scanning;
            if (wasScanning)
            {
                StopAutoScan();
            }

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
            if (wasScanning)
            {
                StartAutoScan();
            }
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
                    if ((myStream = ofd.OpenFile()) != null)
                    {
                        imagePath = ofd.FileName;
                    }
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
