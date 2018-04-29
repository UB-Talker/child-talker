using System;
using System.Collections.Generic;
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
        private IChildTalkerTile backItem, addItemItem, addFolderItem;
        private List<IChildTalkerTile> currentChildren;
        private string ProfilePath;

        public PageViewer()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            ProfilePath = "../../Resources/example2.xml";

            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.jpg", this);
            addItemItem = new ChildTalkerTileAdder("Click to Add Item", "../../Resources/whelpegg.png", this);
            addFolderItem = new ChildTalkerFolderAdder("Click to Add Folder", "../../Resources/whelpegg.png", this);
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
            ChildTalkerXmlWrapper wrapper;
            using (XmlReader reader = XmlReader.Create(_path))
            {
                wrapper = (ChildTalkerXmlWrapper) serializer.Deserialize(reader);
            }

            List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
            foreach (var child in wrapper.Children)
            {
                ctTiles.Add(ParseNode(child));
            }

            AddMultipleItems(ctTiles);
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
                folder.Children = ctTiles;
                return folder;
            }
        }

        public void AddMultipleItems(List<IChildTalkerTile> _ctTiles, Boolean setFromBackButton = false)
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
                    _ctTiles.Add(backItem);
                }
                
            }
            if (!_ctTiles.Contains(addItemItem))
            {
                _ctTiles.Add(addItemItem);
            }
            if (!_ctTiles.Contains(addFolderItem))
            {
                _ctTiles.Add(addFolderItem);
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

            Item ui = new Item();
            ui.SetItem(_ctTileToAdd);
            ui.SetParent(this);
            items.Children.Add(ui);

            currentChildren.Add(_ctTileToAdd);

            if (wasScanning)
            {
                StartAutoScan();
            }
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
    }
}
