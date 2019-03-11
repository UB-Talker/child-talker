using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Speech.Synthesis;

namespace Child_Talker.Utilities
{
    public sealed class TextUtility
    {
        private static TextUtility instance;


        //Instance variables
        private List<Tuple<DateTime, string>> spokenPhrases;
        private SpeechSynthesizer synth;

        private TextUtility()
        {
            //Loads the saved list of speech history from SpeechHistory.txt once the application launches.
            string text = System.IO.File.ReadAllText("../../Utilities/SpeechHistory.txt");
            spokenPhrases = JsonConvert.DeserializeObject<List<Tuple<DateTime, string>>>(text);

            synth = new SpeechSynthesizer();
        }


        /*
         * Static function that is used to get the instance of the TextUtility class
         */
        public static TextUtility Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new TextUtility();
                }
                return instance;
            }
        }

        

        /*
         * Speaks and saves the given input text with a timestamp of when it was spoken.
         */
        public void speak(string text)
        {
            Tuple<DateTime, string> phraseData = new Tuple<DateTime, string>(DateTime.Now, text);
            spokenPhrases.Add(phraseData);

            synth.SpeakAsync(text);
        }


        /*
         * !!!!!! USE ONLY WHEN THE WINDOW IS CLOSING !!!!!!!!!!
         * Writes contents of spokenPhrases to SpeechHistory.txt
         */
        public void save()
        {
            string data = JsonConvert.SerializeObject(spokenPhrases);
            System.IO.File.WriteAllText("../../Utilities/SpeechHistory.txt", data);
        }

    }
}
