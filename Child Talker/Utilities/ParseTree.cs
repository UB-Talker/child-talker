using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker.Utilities
{
    public class ParseTree
    {
        //Maintains the head of the tree structure
        private ParseNode headNode;

        //Used to traverse through the tree
        private ParseNode currentNode;

        //Stores the currently typed string
        private string currentInput;


        /*
         * Build a ParseTree with a given set of words
         */
        public ParseTree(Dictionary<string, int> wordCounts)
        {
            headNode = new ParseNode('_', 0, null);
            currentNode = headNode;
            currentInput = "";

            foreach(KeyValuePair<string, int> pair in wordCounts)
            {
                string s = pair.Key;
                int count = pair.Value;

                foreach(char c in s)
                {
                    if(currentNode.getNextNode(c) == null)
                    {
                        currentNode.setNextNode(c, 0);
                    }
                    currentNode = currentNode.getNextNode(c);
                }

                currentNode.setTimesTerminatedHere(count);
                currentNode = headNode;

            }
        }


        /*
         * Default constructor for ParseTree. Sets the headNode to a new node.
         */
        public ParseTree()
        {
            headNode = new ParseNode('_', 0, null);
            currentNode = headNode;
            currentInput = "";
        }





        /*
         * Traverse down the tree to the node at the given char.
         */
        public void goDownTree(char c)
        {
            currentNode = currentNode.getNextNode(c);
            currentInput += c;
        }


        /*
         * Traverse up the tree. Used primarily when the user backspaces.
         */
        public void goUpTree()
        {
            if(currentNode != headNode)
            {
                currentNode = currentNode.getPrevNode();
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
        }


        /*
         * Returns how many times the currentNode has been terminated there
         */
        public int getCurrentNodeCount()
        {
            return currentNode.getTimesTerminatedHere();
        }



        /*
         * Sets the currentNode to headNode
         */
        public void goToHead()
        {
            currentNode = headNode;
            currentInput = "";
        }


        /*
         * Made this method for testing purposes.
         * May have applicable use.
         */
        public bool isCurrNull()
        {
            return currentNode == null;
        }



        /*
         * Generates the list of suggested words based on the current input.
         * Any strings that have 0 occurences are not added to the list.
         */
        public List<KeyValuePair<string, int>> getSuggestions()
        {
            List<KeyValuePair<string, int>> suggestions = new List<KeyValuePair<string, int>>();

            foreach(ParseNode n in currentNode.getNodes())
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
                s += node.getChar();
                int count = node.getTimesTerminatedHere();
                if (count > 0)
                {
                    KeyValuePair<string, int> pair = new KeyValuePair<string, int>(s, count);
                    accum.Add(pair);
                }
            }

            foreach (ParseNode n in node.getNodes())
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

                for(char i = 'a'; i <= 'z'; i++)
                {
                    followingChars.Add(i, null);
                }

            }


            /*
             * Access the next node at the given char
             * Used to navigate down the tree
             */
            public ParseNode getNextNode(char c)
            {
                return followingChars[c];
            }


            /*
             * Used to set a node in the dictionary to a brand new node.
             * Should only be done if the node is null
             */
            public void setNextNode(char c, int count)
            {
                followingChars[c] = new ParseNode(c, count, this);
            }


            /*
             * Used to change the number of times a parse has terminated at that node
             */
            public void setTimesTerminatedHere(int count)
            {
                timesTerminatedHere = count;
            }


            /*
             * Returns the number of times that a string has terminated at this node
             */
            public int getTimesTerminatedHere()
            {
                return timesTerminatedHere;
            }


            /*
             * Get the char stored at this node
             */
            public char getChar()
            {
                return c;
            }


            /*
             * Get list of ParseNodes stored at this node
             */
            public Dictionary<char, ParseNode>.ValueCollection getNodes()
            {
                return followingChars.Values;
            }


            public ParseNode getPrevNode()
            {
                return parent;
            }
        }
    }
}
