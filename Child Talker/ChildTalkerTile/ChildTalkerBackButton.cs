using Child_Talker.TalkerViews;

namespace Child_Talker
{
    class ChildTalkerBackButton : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        private readonly PageViewer root;
        private readonly Utilities.Autoscan scan;

        public ChildTalkerBackButton(string _text, string _imagePath, PageViewer _root)
        {
            Text = _text;
            ImagePath = _imagePath;
            root = _root;
            Xml = null;
            //Xml.Text = null;
            //Xml.ImagePath = null;
            scan = Utilities.Autoscan.instance;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            root.PopFolderView();
            if (scan.isScanning())
            {
                scan.StartAutoscan<Item>(root.items);
            }
        }
    }
}
