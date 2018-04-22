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
        public UriKind UriKind { get; set; }
        public List<IChildTalkerTile> Children = new List<IChildTalkerTile>();
        public PageViewer Root;
        public IChildTalkerTile Parent { get; set; }

        public ChildTalkerFolder(string text, string imagePath, List<IChildTalkerTile> children, PageViewer root, UriKind uriKind = UriKind.Relative)
        {
            Text = text;
            ImagePath = imagePath;
            UriKind = uriKind;
            SetChildren(children);
            Root = root;
        }

        public ChildTalkerFolder(string text, string imagePath, PageViewer parent)
        {
            Text = text;
            ImagePath = imagePath;
            Root = parent;
        }

        public bool IsLink()
        {
            throw new NotImplementedException();
        }

        public void PerformAction()
        {
            Root.SetItems(Children);
        }

        public void SetChildren(List<IChildTalkerTile> children)
        {
            Children = children;

            foreach (IChildTalkerTile child in Children)
            {
                child.Parent = this;
            }
        }
    }
}
