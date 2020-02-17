using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Media;
using Child_Talker.TalkerButton;
using Button = Child_Talker.TalkerButton.Button;

namespace Child_Talker.Utilities
{
    public sealed class TextUtility
    {
        private static TextUtility _instance;
        /// <summary>
        /// TextUtility as Unary Class.
        /// maintains a single instance of TextUtility for all classes to use
        /// </summary>
        public static TextUtility Instance => _instance ?? (_instance = new TextUtility());


        //Instance variables
        private List<Tuple<DateTime, string>> spokenPhrases;
        private Dictionary<string, int> wordCounts;
        private SpeechSynthesizer synth;
        private ParseTree parseTree;

        private TextUtility()
        {
            //Loads the saved list of speech history from SpeechHistory.txt once the application launches.

            string spokenPhrasesText = "";
            string wordCountText = "";

            if (Directory.Exists(App.StartupPath + "/Utilities"))
            {

                if (!File.Exists(App.StartupPath + "/Utilities/SpeechHistory.txt"))
                {
                    File.Create(App.StartupPath + "/Utilities/SpeechHistory.txt");
                }
                else
                {
                    spokenPhrasesText = File.ReadAllText(App.StartupPath + "/Utilities/SpeechHistory.txt");
                }

                if (!File.Exists(App.StartupPath + "/Utilities/WordCount.txt"))
                {
                    File.Create(App.StartupPath + "/Utilities/WordCount.txt");
                }
                else
                {
                    wordCountText = File.ReadAllText(App.StartupPath + "/Utilities/WordCount.txt");
                }

                spokenPhrases = JsonConvert.DeserializeObject<List<Tuple<DateTime, string>>>(spokenPhrasesText);
                wordCounts = JsonConvert.DeserializeObject<Dictionary<string, int>>(wordCountText);
            }

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




        
        /// <summary>
        /// Speaks and saves the given input text with a timestamp of when it was spoken.
        /// The spoken string is also tokenized and stored in wordCounts. If the string
        /// already exists as a key in wordCounts, its mapped value will be incremented by one.
        /// Otherwise it will be added to wordCounts with an initial value of 1.
        /// </summary>
        /// <param name="text"></param>
        public void Speak(string text)
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
            try
            {

                string speechHistoryData = JsonConvert.SerializeObject(spokenPhrases);
                File.WriteAllText(App.StartupPath + "Utilities/SpeechHistory.txt", speechHistoryData);

                string wordCountData = JsonConvert.SerializeObject(wordCounts);
                File.WriteAllText(App.StartupPath + "Utilities/WordCount.txt", wordCountData);
            }

            catch { }
            
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
        public List<Button> getNextSuggestion(char c)
        {
            if (c == '_')
            {
                parseTree.goUpTree();
            }
            else
            {
                parseTree.GoDownTree(char.ToLower(c));
            }
            return getNSuggestions(10);
        }

        public List<Button> getNextSuggestionsForString(string s)
        {
            if (parseTree.IsTreeReset())
            {
                parseTree.GoDownTree(s);
            }
            return getNSuggestions(3);
        }
        /*
         * Generate n buttons that will be used as suggestions
         */
        private List<Button> getNSuggestions(int n)
        {
            List<Button> retVal = new List<Button>();
            List<KeyValuePair<string, int>> suggestions = parseTree.getSuggestions();

            int i = 0;
            while (i < n && i < suggestions.Count)
            {
                Button b = new Button();
                BrushConverter bc = new BrushConverter();
                b.Foreground = (Brush)bc.ConvertFrom("#FF00D5EA");
                TextBlock textBlock = new TextBlock
                {
                    Text = suggestions[i].Key
                };
                b.Text = suggestions[i].Key;
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
            parseTree.ResetTree();
        }
    }
}
