using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Child_Talker
{
    class ChildTalkerFolderAdder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        private PageViewer Root;
        private String FolderImgPath = "../../Resources/folder.jpg";

        public ChildTalkerFolderAdder(string text, string imagePath, PageViewer root)
        {
            Text = text;
            ImagePath = imagePath;
            Root = root;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            String inputPhrase = Interaction.InputBox("Enter the phrase you would like to add", "Enter Phrase", "Hello World!");
            if (inputPhrase != "")
            {
                    ChildTalkerFolder ctFolder = new ChildTalkerFolder(inputPhrase, FolderImgPath, Root);
                    Root.AddSingleItem(ctFolder);
            }
        }
    }
}
