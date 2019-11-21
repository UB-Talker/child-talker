using Child_Talker.TalkerViews;
using Child_Talker.TalkerViews.PhrasesPage;
using Child_Talker.Utilities.Autoscan;

namespace Child_Talker
{
    class ChildTalkerBackButton : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public bool InColor { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        private readonly Phrases root;
        private readonly Autoscan2 scan;

        public ChildTalkerBackButton(string _text, string _imagePath, Phrases _root, bool _inColor)
        {
            Text = _text;
            ImagePath = _imagePath;
            InColor = false;
            root = _root;
            Xml = null;
            //Xml.Text = null;
            //Xml.ImagePath = null;
            scan = Autoscan2.Instance;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            root.PopFolderView();
            scan.NewListToScanThough<PhraseButton>(root.items);
        }
    }
}
