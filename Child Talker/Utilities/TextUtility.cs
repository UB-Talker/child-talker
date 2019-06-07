using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace Child_Talker.Utilities
{
    public sealed class TextUtility
    {
        private static TextUtility instance;


        //Instance variables
        private List<Tuple<DateTime, string>> spokenPhrases;
        private Dictionary<string, int> wordCounts;
        private SpeechSynthesizer synth;
        private ParseTree parseTree;

        private TextUtility()
        {
            //Loads the saved list of speech history from SpeechHistory.txt once the application launches.

            string spokenPhrasesText = "";
            if (!File.Exists("../../Utilities/SpeechHistory.txt")) { 
                File.Create("../../Utilities/SpeechHistory.txt");
            } else {
                spokenPhrasesText = File.ReadAllText("../../Utilities/SpeechHistory.txt");
            }

            string wordCountText = "";
            if (!File.Exists("../../Utilities/WordCount.txt")) {
                File.Create("../../Utilities/WordCount.txt");
            } else {
                wordCountText = File.ReadAllText("../../Utilities/WordCount.txt");
            }

            spokenPhrases = JsonConvert.DeserializeObject<List<Tuple<DateTime, string>>>(spokenPhrasesText);
            wordCounts = JsonConvert.DeserializeObject<Dictionary<string, int>>(wordCountText);

            if(spokenPhrases == null)
            {
                spokenPhrases = new List<Tuple<DateTime, string>>();
            }

            if(wordCounts == null)
            {
                wordCounts = new Dictionary<string, int>();
            }

            parseTree = new ParseTree(wordCounts);
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
            text = removePunctuation(text);
            string[] words = text.Split(' ');

            foreach (string word in words)
            {
                string _word = word.ToLower();
                if (wordCounts.ContainsKey(_word))
                {
                    wordCounts[_word] += 1;
                }
                else
                {
                    wordCounts.Add(_word, 1);
                }
                parseTree.addString(_word);

            }
        }



        private string removePunctuation(string s)
        {
            string empty = "";
            string[] punctuation = {".", ","};
            
            foreach(string p in punctuation)
            {
                s = s.Replace(p, empty);
            }

            return s;
        }
        
        /*
         * !!!!!! USE ONLY WHEN THE WINDOW IS CLOSING !!!!!!!!!!
         * Writes contents of spokenPhrases to SpeechHistory.txt
         * Writes contents of wordCounts to WordCount.txt
         */
        public void save()
        {
            string speechHistoryData = JsonConvert.SerializeObject(spokenPhrases);
            File.WriteAllText("../../Utilities/SpeechHistory.txt", speechHistoryData);

            string wordCountData = JsonConvert.SerializeObject(wordCounts);
            File.WriteAllText("../../Utilities/WordCount.txt", wordCountData);
            
        }


        /*
         * Returns a list of the spokenPhrases. Currently used in the History page to 
         * display the list of spoken phrases.
         */
        public List<Tuple<DateTime, string>> getSpokenPhrases()
        {
            return spokenPhrases;
        }


        /*
         * Gets the next suggestions based on the given input c
         */
        public List<TlkrBTN> getNextSuggestion(char c)
        {
            if (c == '_')
            {
                parseTree.goUpTree();
            }
            else
            {
                parseTree.goDownTree(char.ToLower(c));
            }
            return getNSuggestions(10);
        }

        public List<TlkrBTN> getNextSuggestionsForString(string s)
        {
            if (parseTree.isTreeReset())
            {
                parseTree.goDownTree(s);
            }
            return getNSuggestions(3);
        }
        /*
         * Generate n buttons that will be used as suggestions
         */
        private List<TlkrBTN> getNSuggestions(int n)
        {
            List<TlkrBTN> retVal = new List<TlkrBTN>();
            List<KeyValuePair<string, int>> suggestions = parseTree.getSuggestions();

            int i = 0;
            while (i < n && i < suggestions.Count)
            {
                TlkrBTN b = new TlkrBTN();
                BrushConverter bc = new BrushConverter();
                b.Foreground = (Brush)bc.ConvertFrom("#FF00D5EA");
                TextBlock textBlock = new TextBlock
                {
                    Text = suggestions[i].Key
                };
                b.Tag = suggestions[i].Key;
                b.Content = textBlock;
                retVal.Add(b);
                i++;
            }

            return retVal;
        }


        /*
         * Resets the autocorrect when necessary
         */
        public void resetAutocorrect()
        {
            parseTree.resetTree();
        }
    }
}
