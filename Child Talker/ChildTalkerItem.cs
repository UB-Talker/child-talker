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

        public ChildTalkerItem(string text, string imagePath)
        {
            Text = text;
            ImagePath = imagePath;
        }

        public bool IsLink()
        {
            return Children.Count != 0;
        }
    }
}
