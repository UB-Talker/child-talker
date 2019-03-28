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
        private Dictionary<string, int> wordCounts;
        private SpeechSynthesizer synth;

        private TextUtility()
        {
            //Loads the saved list of speech history from SpeechHistory.txt once the application launches.
            string spokenPhrasesText = System.IO.File.ReadAllText("../../Utilities/SpeechHistory.txt");
            string wordCountText = System.IO.File.ReadAllText("../../Utilities/WordCount.txt");
            spokenPhrases = JsonConvert.DeserializeObject<List<Tuple<DateTime, string>>>(spokenPhrasesText);
            wordCounts = JsonConvert.DeserializeObject<Dictionary<string, int>>(wordCountText);

            if(wordCounts == null)
            {
                wordCounts = new Dictionary<string, int>();
            }

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
         * The spoken string is also tokenized and stored in wordCounts. If the string
         * already exists as a key in wordCounts, its mapped value will be incremented by one.
         * Otherwise it will be added to wordCounts with an initial value of 1.
         */
        public void speak(string text)
        {
            Tuple<DateTime, string> phraseData = new Tuple<DateTime, string>(DateTime.Now, text);
            spokenPhrases.Add(phraseData);

            synth.SpeakAsync(text);

            string[] words = text.Split(' ');
            
            for(int i  = 0; i < words.Length; i++)
            {
                string currentWord = words[i].ToLower();
                if (wordCounts.ContainsKey(currentWord))
                {
                    wordCounts[currentWord] += 1;
                }
                else
                {
                    wordCounts.Add(currentWord, 1);
                }
            }
        }


        /*
         * !!!!!! USE ONLY WHEN THE WINDOW IS CLOSING !!!!!!!!!!
         * Writes contents of spokenPhrases to SpeechHistory.txt
         * Writes contents of wordCounts to WordCount.txt
         */
        public void save()
        {
            string speechHistoryData = JsonConvert.SerializeObject(spokenPhrases);
            System.IO.File.WriteAllText("../../Utilities/SpeechHistory.txt", speechHistoryData);

            string wordCountData = JsonConvert.SerializeObject(wordCounts);
            System.IO.File.WriteAllText("../../Utilities/WordCount.txt", wordCountData);
        }

    }
}
