using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker
{
    class ChildTalkerFolder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public List<IChildTalkerTile> Children = new List<IChildTalkerTile>();
        private List<ChildTalkerXml> XmlChildren = new List<ChildTalkerXml>();
        public PageViewer Root;
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        public ChildTalkerFolder(string _text, string _imagePath, PageViewer _root, List<IChildTalkerTile> _children = null)
        {
            Text = _text;
            ImagePath = _imagePath;
            SetChildren(_children);
            Root = _root;
            Xml = new ChildTalkerXml();
            Xml.Text = Text;
            Xml.ImagePath = ImagePath;
            Xml.Children = XmlChildren;
        }

        public bool IsLink()
        {
            return true;
        }

        public void PerformAction()
        {
            Root.AddMultipleItems(Children);
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
                    XmlChildren.Add(child.Xml);
                }
            }
        }
    }
}
