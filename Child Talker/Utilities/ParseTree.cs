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

        //Stores the last non null node
        private ParseNode nonNullNode;

        //Stores the currently typed string
        private string currentInput;

        //Store how many times the currentNode should remain null
        //i.e. g->o->o->d->null is stored in the tree and the user types "goodbye"
        //nullNodes will be 3
        private int nullNodes;

        //Indicates whether or not the ParseTree is reset
        private bool isReset;


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
            isReset = true;
           
        }


        /*
         * Default constructor for ParseTree. Sets the headNode to a new node.
         */
        public ParseTree()
        {
            headNode = new ParseNode('_', 0, null);
            currentNode = headNode;
            currentInput = "";
            isReset = true;
        }



        /*
         * Traverse down the tree to the node at the given char.
         */
        public void goDownTree(char c)
        {
            isReset = false;

            if(c >= '0' && c <= '9')
            {
                if(currentNode != null)
                {
                    nonNullNode = currentNode;
                }
                currentNode = null;
            }

            else if (currentNode != null)
            {
                nonNullNode = currentNode;
                currentNode = currentNode.getNextNode(c);
            }

            if(currentNode == null)
            {
                nullNodes += 1;
            }

            currentInput += c;
        }


        /*
         * Traverse down the tree to the node with the given string
         */
        public void goDownTree(string s)
        {
            s = s.ToLower();
            foreach(char c in s)
            {
                goDownTree(c);
            }
        }


        /*
         * Traverse up the tree. Used primarily when the user backspaces.
         */
        public void goUpTree()
        {
            if(currentNode != headNode)
            {
                nullNodes -= 1;
                if (nullNodes <= 0)
                {
                    if (currentNode == null)
                    {
                        currentNode = nonNullNode;
                    }
                    else
                    {
                        currentNode = currentNode.getPrevNode();
                    }
                    nullNodes = 0;
                }
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }

            if(currentNode == headNode)
            {
                isReset = true;
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
        public void resetTree()
        {
            currentNode = headNode;
            currentInput = "";
            nullNodes = 0;
            isReset = true;
        }

        /*
         * Returns if true if the tree is reset.
         * In other words if the currentNode is at the head.
         */
        public bool isTreeReset()
        {
            return isReset;
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
         * When a string is spoken, each word is added to the parse tree
         */
        public void addString(string s)
        {
            currentNode = headNode;
            foreach(char c in s)
            {
                if(currentNode.getNextNode(c) == null)
                {
                    currentNode.setNextNode(c, 0);
                }
                currentNode = currentNode.getNextNode(c);
            }
            currentNode.incrementCount();
            resetTree();
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


            /*
             * Traverse to the parent node
             */
            public ParseNode getPrevNode()
            {
                return parent;
            }


            /*
             * Increments the count of the current node by one
             */
            public void incrementCount()
            {
                timesTerminatedHere++;
            }
        }
    }
}
