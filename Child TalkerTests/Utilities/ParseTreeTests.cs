using Microsoft.VisualStudio.TestTools.UnitTesting;
using Child_Talker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker.Utilities.Tests
{
    /*
     * Test suite that aims to verify the ParseTree class is working as intended
     */
    [TestClass()]
    public class ParseTreeTests
    {
        /*
         * Test that the tree is constructed properly
         */
        [TestMethod()]
        public void ParseTreeTestConstruction()
        {
            Dictionary<string, int> words = new Dictionary<string, int>();
            words.Add("hello", 4);
            words.Add("the", 2);

            ParseTree tree = new ParseTree(words);

            tree.goDownTree('h');
            Assert.AreEqual(tree.getCurrentNodeCount(), 0);

            tree.goDownTree('e');
            Assert.AreEqual(tree.getCurrentNodeCount(), 0);

            tree.goDownTree('l');
            Assert.AreEqual(tree.getCurrentNodeCount(), 0);

            tree.goDownTree('l');
            tree.goDownTree('o');

            Assert.AreEqual(tree.getCurrentNodeCount(), 4);

            tree.goToHead();

            tree.goDownTree('t');tree.goDownTree('h');tree.goDownTree('e');
            Assert.AreEqual(tree.getCurrentNodeCount(), 2);

            tree.goDownTree('x');
            Assert.IsTrue(tree.isCurrNull());
        }


        /*
         * Tests that the ParseTree is generating the proper suggestions
         */
        [TestMethod()]
        public void TestSuggestions()
        {
            Dictionary<string, int> words = new Dictionary<string, int>();
            words.Add("hello", 4);
            words.Add("the", 2);

            ParseTree tree = new ParseTree(words);

            List<KeyValuePair<string, int>> suggestions = tree.getSuggestions();
            List<KeyValuePair<string, int>> answer = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("hello", 4),
                new KeyValuePair<string, int>("the", 2)
            };

            //Check that "hello" is the top suggestion and "the" is the second
            Assert.AreEqual(2, suggestions.Count);
            Assert.AreEqual(answer[0], suggestions[0]);
            Assert.AreEqual(answer[1], suggestions[1]);

            //Check that the suggestions no longer considers "the" as a possible string after 'h' is input
            tree.goDownTree('h');
            suggestions = tree.getSuggestions();
            Assert.AreEqual(1, suggestions.Count);
            Assert.AreEqual(answer[0], suggestions[0]);

            //Check that goUpTree works properly and "the" is reconsidered as a suggestion
            tree.goUpTree();
            suggestions = tree.getSuggestions();
            Assert.AreEqual(answer.Count, suggestions.Count);
            Assert.AreEqual(answer[0], suggestions[0]);
            Assert.AreEqual(answer[1], suggestions[1]);

            //Check that "hello" is no longer considered as a suggestion after 't' is input
            tree.goToHead();
            tree.goDownTree('t');
            suggestions = tree.getSuggestions();
            Assert.AreEqual(1, suggestions.Count);
            Assert.AreEqual(answer[1], suggestions[0]);
        }
    }
}