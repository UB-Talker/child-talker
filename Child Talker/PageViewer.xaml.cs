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
        private IChildTalkerTile backItem, addItemItem;
        private List<IChildTalkerTile> currentChildren;

        public PageViewer()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);

            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.jpg", this);
            addItemItem = new ChildTalkerItemAdder("Click to Add Item", "../../Resources/whelpegg.png", this);
        }

        public void StartAutoScan()
        {
            scanning = true;
            i = 0;
            ((Item)this.items.Children[i]).AutoSelected = true;
            timer.Start();
        }

        public void StopAutoScan()
        {
            scanning = false;
            if (this.items.Children.Count != 0)
            {
                ((Item)this.items.Children[i]).AutoSelected = false;
            }

            timer.Stop();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                Console.WriteLine($"{i} / {items.Children.Count}");
                ((Item)this.items.Children[i]).AutoSelected = false;
                i = (i + 1) % this.items.Children.Count;
                ((Item)this.items.Children[i]).AutoSelected = true;
            });
        }

        public void LoadFromXml(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode node = doc.DocumentElement.SelectSingleNode("/profile/items");
            List<IChildTalkerTile> items = new List<IChildTalkerTile>();
            foreach (XmlNode item in node.ChildNodes)
            {
                items.Add(LoadItem(item));
            }

            SetItems(items);
        }

        public IChildTalkerTile LoadItem(XmlNode node)
        {
            IChildTalkerTile item;
            if (node.ChildNodes.Count != 0 )
            {
                item = new ChildTalkerFolder(node.Attributes["name"].InnerText, node.Attributes["image"].InnerText, this);
                Console.WriteLine(item.Text);
            }
            else
            {
                item = new ChildTalkerItem(node.Attributes["name"].InnerText, node.Attributes["image"].InnerText);
                Console.WriteLine(item.Text);
            }
            
            List<IChildTalkerTile> children = new List<IChildTalkerTile>();
            foreach (XmlNode child in node.ChildNodes)
            {
                children.Add(LoadItem(child));
            }

            if (children.Count != 0)
            {
               
                ((ChildTalkerFolder)item).SetChildren(children);
            }

            return item;
        }

        public void SetItems(List<IChildTalkerTile> items, Boolean setFromBackButton = false)
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
                if (!items.Contains(backItem))
                {
                    items.Add(backItem);
                }
                
            }
            if (!items.Contains(addItemItem))
            {
                items.Add(addItemItem);
            }

            currentChildren = items;
            this.items.Children.Clear();
            foreach (IChildTalkerTile item in items)
            {
                Item ui = new Item();
                ui.SetItem(item);
                ui.SetParent(this);
                this.items.Children.Add(ui);
            }

            if (wasScanning)
            {
                StartAutoScan();
            }
        }

        public void AddSingleItem(IChildTalkerTile itemToAdd)
        {
            bool wasScanning = scanning;
            if (wasScanning)
            {
                StopAutoScan();
            }

            Item ui = new Item();
            ui.SetItem(itemToAdd);
            ui.SetParent(this);
            this.items.Children.Add(ui);

            currentChildren.Add(itemToAdd);

            if (wasScanning)
            {
                StartAutoScan();
            }
        }

        public void PopFolderView()
        {
            if (folderTrace.Count > 0)
            {
                SetItems(folderTrace.Pop(), true);
            }
        }
    }
}
