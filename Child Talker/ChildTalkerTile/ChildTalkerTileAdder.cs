using Child_Talker.TalkerViews;
using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using Child_Talker.TalkerViews.PhrasesPage;

namespace Child_Talker
{
    class ChildTalkerTileAdder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }
        public bool HasColor { get; set; }

        private Phrases Root;

        public ChildTalkerTileAdder(string _text, string _imagePath, Phrases _root)
        {
            Text = _text;
            ImagePath = _imagePath;
            HasColor = IsFileTypeTransparent(_imagePath);
            Root = _root;
            // collection initializer
            Xml = new ChildTalkerXml()
            {
                Text = _text,
                ImagePath = _imagePath,
                TileType = ChildTalkerXml.Tile.talker
            };
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
                String imagePath = PromptFileExplorer();
                if (imagePath != "")
                {
                    ChildTalkerTile ctItem = new ChildTalkerTile(inputPhrase, imagePath, IsFileTypeTransparent(imagePath));
                    Root.AddSingleItem(ctItem);
                }
            }
        }

        public static bool IsFileTypeTransparent(String s)
        {
            int exe = s.LastIndexOf('.');
            return !s.Substring(exe+1).Contains("png");
        }

        private String PromptFileExplorer()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;

            String imagePath = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {   
                try
                {
                    imagePath = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            return imagePath;
        }
    }
}
