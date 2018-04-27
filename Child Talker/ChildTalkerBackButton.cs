﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker
{
    class ChildTalkerBackButton : IChildTalkerTile
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public UriKind UriKind { get; set; }
        public IChildTalkerTile Parent { get; set; }
        private PageViewer Root;

        public ChildTalkerBackButton(string text, string imagePath, PageViewer root, UriKind uriKind = UriKind.Relative)
        {
            Text = text;
            ImagePath = imagePath;
            UriKind = uriKind;
            Root = root;
        }

        public bool IsLink()
        {
            return false;
        }

        public void PerformAction()
        {
            Root.PopFolderView();
        }
    }
}