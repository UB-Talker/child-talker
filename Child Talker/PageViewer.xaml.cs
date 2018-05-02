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

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for PageViewer.xaml
    /// </summary>
    public partial class PageViewer : UserControl
    {
        private Timer timer;
        private int i = 0;
        private bool scanning = false;
        private Stack<List<IChildTalkerTile>> folderTrace = new Stack<List<IChildTalkerTile>>();
        private ChildTalkerXmlWrapper XmlWrapper;
        private IChildTalkerTile backItem;
        private List<IChildTalkerTile> currentChildren;
        private string ProfilePath;
        public Stack<ChildTalkerFolder> ViewParents = new Stack<ChildTalkerFolder>();
        private List<IChildTalkerTile> RootChildren = new List<IChildTalkerTile>();

        public PageViewer()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            ProfilePath = "../../Resources/example2.xml";
            scanning = false;

            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.jpg", this);
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
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            //ChildTalkerXmlWrapper wrapper;
            using (XmlReader reader = XmlReader.Create(_path))
            {
                XmlWrapper = (ChildTalkerXmlWrapper) serializer.Deserialize(reader);
            }

            List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
            foreach (var child in XmlWrapper.Children)
            {
                ctTiles.Add(ParseNode(child));
            }

            AddMultipleItems(ctTiles, false, true);
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
            if (_node.Children.Count == 0)
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

        public void AddMultipleItems(List<IChildTalkerTile> _ctTiles, Boolean setFromBackButton = false, Boolean calledFromLoad = false)
        {
            bool wasScanning = scanning;
            if (wasScanning)
            {
                StopAutoScan();
            }

            if (currentChildren != null)
            {
                if (!setFromBackButton)
                {
                    folderTrace.Push(currentChildren);
                }
                if (!_ctTiles.Contains(backItem) && folderTrace.Count > 0)
                {
                    _ctTiles.Insert(0, backItem);
                }  
            }

            currentChildren = _ctTiles;
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
                RootChildren = _ctTiles;
                XmlWrapper.Children = new List<ChildTalkerXml>();
                foreach (IChildTalkerTile rootChild in RootChildren)
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
           
            if (false)
            {
                StopAutoScan();
            }

            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().AddChild(_ctTileToAdd);
            }
            else
            {
                RootChildren.Add(_ctTileToAdd);
                XmlWrapper.Children.Add(_ctTileToAdd.Xml);
            }
            Item ui = new Item();
            ui.SetItem(_ctTileToAdd);
            ui.SetParent(this);
            items.Children.Add(ui);

            currentChildren.Add(_ctTileToAdd);

            SaveToXml(ProfilePath);
            if (false)
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
            currentChildren.Remove(_itemToRemove.CtTile);
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().RemoveChild(_itemToRemove.CtTile);
            }
            else
            {
                RootChildren.Remove(_itemToRemove.CtTile);
            }

            SaveToXml(ProfilePath);
            if (wasScanning)
            {
                StartAutoScan();
            }
        }

        public void PopFolderView()
        {
            if (folderTrace.Count > 0)
            {
                AddMultipleItems(folderTrace.Pop(), true);
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
