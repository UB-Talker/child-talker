using Child_Talker.TalkerViews;
using System.Collections.Generic;

namespace Child_Talker
{
    public class ChildTalkerFolder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public List<IChildTalkerTile> Children = new List<IChildTalkerTile>();
        private List<ChildTalkerXml> XmlChildren = new List<ChildTalkerXml>();
        public PageViewer Root;
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        private Utilities.Autoscan scan;

        public ChildTalkerFolder(string _text, string _imagePath, PageViewer _root, List<IChildTalkerTile> _children = null)
        {
            Text = _text;
            ImagePath = _imagePath;
            SetChildren(_children);
            Root = _root;
            Xml = new ChildTalkerXml
            {
                Text = _text,
                ImagePath = _imagePath,
                TileType = ChildTalkerXml.Tile.folder,
                Children = XmlChildren
            };
            /*
             * Used Collection initializer instead
             * Xml.Text = Text;
             * Xml.ImagePath = ImagePath;
             * Xml.TileType = ChildTalkerXml.Tile.folder;
             * Xml.Children = XmlChildren;
             */
            scan = Utilities.Autoscan.instance;

        }

        public bool IsLink()
        {
            return true;
        }

        public void PerformAction()
        {
            Root.ViewParents.Push(this);
            Root.LoadTiles(Children);
            if (scan.isScanning())
            {
                scan.StartAutoscan<Item>(Root.items);
            }
            
        }

        public void SetChildren(List<IChildTalkerTile> _children)
        {
            if (_children != null)
            {
                Children = _children;
                XmlChildren.Clear();

                foreach (IChildTalkerTile child in Children)
                {
                    child.Parent = this;
                    if (child.Xml != null)
                    {
                        XmlChildren.Add(child.Xml);
                    }
                }
            }
        }

        public void AddChild(IChildTalkerTile _child)
        {
            if (_child != null)
            {
                _child.Parent = this;
                Children.Add(_child);
                if (_child.Xml != null)
                {
                    XmlChildren.Add(_child.Xml);
                }
            }
        }

        public void RemoveChild(IChildTalkerTile _child)
        {
            if (_child != null)
            {
                _child.Parent = null;
                Children.Remove(_child);
                XmlChildren.Remove(_child.Xml);
            }
        }
    }
}
