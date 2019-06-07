﻿using Child_Talker.TalkerViews;
using Microsoft.VisualBasic;
using System;

namespace Child_Talker
{
    class ChildTalkerFolderAdder : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public IChildTalkerTile Parent { get; set; }
        public ChildTalkerXml Xml { get; set; }

        private readonly PageViewer Root;
        private String FolderImgPath = "../../Resources/folder.jpg";

        public ChildTalkerFolderAdder(string _text, string _imagePath, PageViewer _root)
        {
            Text = _text;
            ImagePath = _imagePath;
            Root = _root;
            // collection initializer
            Xml = new ChildTalkerXml
            {
                Text = _text,
                ImagePath = _imagePath,
                TileType = ChildTalkerXml.Tile.folder
            };
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            String inputPhrase = Interaction.InputBox("Enter name for the folder you would like to add", "Enter Phrase", "Hello World!");
            if (inputPhrase != "")
            {
                    ChildTalkerFolder ctFolder = new ChildTalkerFolder(inputPhrase, FolderImgPath, Root);
                    Root.AddSingleItem(ctFolder);
            }
        }
    }
}
