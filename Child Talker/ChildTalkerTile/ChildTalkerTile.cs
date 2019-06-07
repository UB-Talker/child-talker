using Child_Talker.Utilities;
using System.Timers;

namespace Child_Talker
{

    public class ChildTalkerTile : IChildTalkerTile
    {
        private static readonly TextUtility util = TextUtility.Instance;
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }
        private Timer timer;


        public ChildTalkerTile(string _text, string _imagePath)
        {
            timer = new Timer();
            Text = _text;
            ImagePath = _imagePath;
            timer.Interval = 1000;
            timer.AutoReset = false;
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
            if (!timer.Enabled)
            {
                util.speak(Text);
                timer.Start();
            }
        }
    }
}
