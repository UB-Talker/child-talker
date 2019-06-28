using Child_Talker.Utilities;
using System.Timers;

namespace Child_Talker
{

    public class ChildTalkerTile : IChildTalkerTile
    {
        private static readonly TextUtility util = TextUtility.Instance;
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public bool InColor { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }


        public ChildTalkerTile(string _text, string _imagePath, bool _inColor)
        {
            Text = _text;
            ImagePath = _imagePath;
            InColor = false;
            //collection initializer
            Xml = new ChildTalkerXml()
            {
                Text = _text,
                ImagePath = _imagePath,
                TileType = ChildTalkerXml.Tile.talker
            };
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            util.Speak(Text);
        }
    }
}
