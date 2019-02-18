using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace Child_Talker
{
    
    public class ChildTalkerTile : IChildTalkerTile
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
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
            Xml = new ChildTalkerXml();
            Xml.Text = Text;
            Xml.ImagePath = ImagePath;
            Xml.TileType = ChildTalkerXml.Tile.talker;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            if (!timer.Enabled)
            {
                synth.SpeakAsync(Text);
                timer.Start();
            }
        }
    }
}
