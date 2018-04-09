using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker
{
    public class ChildTalkerItem
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public List<ChildTalkerItem> Children = new List<ChildTalkerItem>();
        public ChildTalkerItem Parent;

        public ChildTalkerItem(string text, string imagePath)
        {
            Text = text;
            ImagePath = imagePath;
        }

        public ChildTalkerItem(string text, string imagePath, List<ChildTalkerItem> children)
        {
            Text = text;
            ImagePath = imagePath;
            SetChildren(children);
        }

        public void SetChildren(List<ChildTalkerItem> children)
        {
            Children = children;

            foreach (ChildTalkerItem child in Children)
            {
                child.Parent = this;
            }
        }

        public bool IsLink()
        {
            return Children.Count != 0;
        }
    }
}
