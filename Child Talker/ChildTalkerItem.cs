using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Child_Talker
{

    public class ChildTalkerItem : IChildTalkerTile
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        private Timer timer;


        public ChildTalkerItem(string text, string imagePath)
        {
            timer = new Timer();
            Text = text;
            ImagePath = imagePath;
            timer.Interval = 1000;
            timer.AutoReset = false;
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
