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
        public PageViewer Root;
        public IChildTalkerTile Parent { get; set; }

        public ChildTalkerFolder(string text, string imagePath, PageViewer root, List<IChildTalkerTile> children = null)
        {
            Text = text;
            ImagePath = imagePath;
            SetChildren(children);
            Root = root;
        }

        public bool IsLink()
        {
            return true;
        }

        public void PerformAction()
        {
            Root.SetItems(Children);
        }

        public void SetChildren(List<IChildTalkerTile> children)
        {
            if (children != null)
            {
                Children = children;

                foreach (IChildTalkerTile child in Children)
                {
                    child.Parent = this;
                }
            }
        }
    }
}
