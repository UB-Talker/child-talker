﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Child_Talker
{
    class ChildTalkerItemAdder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public UriKind UriKind { get; set; }
        public IChildTalkerTile Parent { get; set; }
        private PageViewer Root;

        public ChildTalkerItemAdder(string text, string imagePath, PageViewer root, UriKind uriKind = UriKind.Relative)
        {
            Text = text;
            ImagePath = imagePath;
            UriKind = UriKind;
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
                String imagePath = PromptFileExplorer();
                if (imagePath != "")
                {
                    ChildTalkerItem ctItem = new ChildTalkerItem(inputPhrase, imagePath, UriKind.Absolute);
                    Root.AddSingleItem(ctItem);
                }
            }
        }

        private String PromptFileExplorer()
        {
            Stream myStream = null;
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;

            String imagePath = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {   
                try
                {
                    if ((myStream = ofd.OpenFile()) != null)
                    {
                        imagePath = ofd.FileName;
                    }
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