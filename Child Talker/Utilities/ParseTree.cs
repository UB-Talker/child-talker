using System.Collections.Generic;

namespace Child_Talker.Utilities
{
    public class ParseTree
    {
        ///Maintains the head of the tree structure
        private ParseNode headNode;

        ///Used to traverse through the tree
        private ParseNode currentNode;

        ///Stores the last non null node
        private ParseNode nonNullNode;

        ///Stores the currently typed string
        private string currentInput;

        ///Store how many times the currentNode should remain null
        //i.e. g->o->o->d->null is stored in the tree and the user types "goodbye"
        //nullNodes will be 3
        private int nullNodes;

        ///Indicates whether or not the ParseTree is reset
        private bool isReset;

        /// <summary>
        /// Build a ParseTree with a given set of words
        /// </summary>
        /// <param name="wordCounts"></param>
        public ParseTree(Dictionary<string, int> wordCounts)
        {
            headNode = new ParseNode('_', 0, null);
            currentNode = headNode;
            currentInput = "";

            foreach(KeyValuePair<string, int> pair in wordCounts)
            {
                string s = pair.Key;
                int count = pair.Value;

                foreach(var character in s)
                {
                    if(currentNode.GetNextNode(character) == null)
                    {
                        currentNode.SetNextNode(character, 0);
                    }
                    currentNode = currentNode.GetNextNode(character);
                }
                currentNode.SetTimesTerminatedHere(count);
                currentNode = headNode;
            }
            isReset = true;
        }

        /// <summary>
        /// Default constructor for ParseTree. Sets the headNode to a new node.
        /// </summary>
        public ParseTree()
        {
            headNode = new ParseNode('_', 0, null);
            currentNode = headNode;
            currentInput = "";
            isReset = true;
        }

        /// <summary>
        /// Traverse down the tree to the node at the given char.
        /// </summary>
        /// <param name="c"></param>
        public void GoDownTree(char c)
        {
            isReset = false;

            if (currentNode != null)
            {
                nonNullNode = currentNode;
                currentNode = currentNode.GetNextNode(c);
            }

            if(currentNode == null)
            {
                nullNodes += 1;
            }

            currentInput += c;
        }

        /// <summary>
        /// Traverse down the tree to the node with the given string
        /// </summary>
        /// <param name="s"></param>
        public void GoDownTree(string s)
        {
            s = s.ToLower();
            foreach(char c in s)
            {
                GoDownTree(c);
            }
        }

        /// <summary>
        /// Traverse up the tree. Used primarily when the user backspaces.
        /// </summary>
        public void goUpTree()
        {
            if(currentNode != headNode)
            {
                nullNodes -= 1;
                if (nullNodes <= 0)
                {
                    currentNode = currentNode == null ? nonNullNode : currentNode.GetPrevNode();
                    nullNodes = 0;
                }
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else 
            {
                isReset = true;
            }
        }


        /// <summary>
        /// Returns how many times the currentNode has been terminated there
        /// </summary>
        /// <returns></returns>
        public int GetCurrentNodeCount()
        {
            return currentNode.GetTimesTerminatedHere();
        }



        /// <summary>
        /// Sets the currentNode to headNode
        /// </summary>
        public void ResetTree()
        {
            currentNode = headNode;
            currentInput = "";
            nullNodes = 0;
            isReset = true;
        }

        /// <summary>
        /// Returns if true if the tree is reset.
        /// In other words if the currentNode is at the head.
        /// </summary>
        /// <returns></returns>
        public bool IsTreeReset()
        {
            return isReset;
        }

        /// <summary>
        /// Made this method for testing purposes.
        /// May have applicable use.
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentNodeNull()
        {
            return currentNode == null;
        }


        /*
         * When a string is spoken, each word is added to the parse tree
         */
        public void addString(string s)
        {
            currentNode = headNode;
            foreach(char c in s)
            {
                if(currentNode.GetNextNode(c) == null)
                {
                    currentNode.SetNextNode(c, 0);
                }
                currentNode = currentNode.GetNextNode(c);
            }
            currentNode.IncrementCount();
            ResetTree();
        }



        /*
         * Generates the list of suggested words based on the current input.
         * Any strings that have 0 occurences are not added to the list.
         */
        public List<KeyValuePair<string, int>> getSuggestions()
        {
            List<KeyValuePair<string, int>> suggestions = new List<KeyValuePair<string, int>>();

            if(currentNode == null || currentNode == headNode)
            {
                return suggestions;
            }

            foreach(ParseNode n in currentNode.GetNodes())
            {
                if(n != null)
                {
                    getSuggestionsHelper(n, suggestions, currentInput);
                }
            }

            suggestions.Sort((a, b) => b.Value - a.Value);
            return suggestions;
        }


        /*
         * Recursive helper function for getSuggestions.
         * Modifies a List that is passed by reference.
         */
        private void getSuggestionsHelper(ParseNode node, List<KeyValuePair<string, int>> accum, string s)
        {
            if (node != headNode)
            {
                s += node.GetChar();
                int count = node.GetTimesTerminatedHere();
                if (count > 0)
                {
                    KeyValuePair<string, int> pair = new KeyValuePair<string, int>(s, count);
                    accum.Add(pair);
                }
            }

            foreach (ParseNode n in node.GetNodes())
            {
                if(n != null)
                {
                    getSuggestionsHelper(n, accum, s);
                }
            }
        }



        /*
         * Define node that will make up the ParseTree structure.
         * This is only accessible by the ParseTree class
         */
        private class ParseNode
        {
            private char c;
            private int timesTerminatedHere;
            private Dictionary<char, ParseNode> followingChars;
            private ParseNode parent;


            public ParseNode(char c, int timesTerminatedHere, ParseNode parent)
            {
                this.c = c;
                this.timesTerminatedHere = timesTerminatedHere;
                this.parent = parent;

                followingChars = new Dictionary<char, ParseNode>();
            }


            /*
             * Access the next node at the given char
             * Used to navigate down the tree
             */
            public ParseNode GetNextNode(char c)
            {
                return followingChars.ContainsKey(c) ? followingChars[c] : null;
            }

            /// <summary>
            /// Used to set a node in the dictionary to a brand new node.
            /// Should only be done if the node is null
            /// </summary>
            /// <param name="c"></param>
            /// <param name="count"></param>
            public void SetNextNode(char c, int count)
            {
                if (!followingChars.ContainsKey(c))
                {
                    followingChars.Add(c, new ParseNode(c, count, this));
                }
            }



            
            
            /// <summary>
            /// Used to change the number of times a parse has terminated at that node
            /// </summary>
            /// <param name="count"></param>
            public void SetTimesTerminatedHere(int count)
            {
                timesTerminatedHere = count;
            }


            /// Returns the number of times that a string has terminated at this node
            public int GetTimesTerminatedHere()
            {
                return timesTerminatedHere;
            }


            /// Get the char stored at this node
            public char GetChar()
            {
                return c;
            }

            /// Get list of ParseNodes stored at this node
            public Dictionary<char, ParseNode>.ValueCollection GetNodes()
            {
                return followingChars.Values;
            }

            /// Traverse to the parent node
            public ParseNode GetPrevNode()
            {
                return parent;
            }

            /// Increments the count of the current node by one
            public void IncrementCount()
            {
                timesTerminatedHere++;
            }
        }
    }
}
