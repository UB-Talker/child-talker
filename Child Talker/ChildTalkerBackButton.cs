using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker
{
    class ChildTalkerBackButton : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        private PageViewer Root;

        public ChildTalkerBackButton(string _text, string _imagePath, PageViewer _root)
        {
            Text = _text;
            ImagePath = _imagePath;
            Root = _root;
            Xml = null;
            //Xml.Text = null;
            //Xml.ImagePath = null;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            Root.PopFolderView();
        }
    }
}
