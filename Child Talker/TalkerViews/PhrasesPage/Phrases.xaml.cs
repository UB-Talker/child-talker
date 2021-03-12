using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Child_Talker.Utilities.Autoscan;

using Child_Talker.TalkerViews.Keyboard;
using Child_Talker.Utilities;
using Timer = System.Timers.Timer;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for PhrasesPage.xaml
    /// </summary>
    public partial class Phrases : TalkerView
    {
        private ChildTalkerXmlWrapper _xmlWrapper;
        private IChildTalkerTile _backItem;
        private static string _profilePath = App.StartupPath + "/Properties/PhraseLayout.xml";
        private List<IChildTalkerTile> _rootChildren = new List<IChildTalkerTile>();
        public Stack<ChildTalkerFolder> ViewParents = new Stack<ChildTalkerFolder>();

        private static readonly Autoscan2 Scan = Autoscan2.Instance;
        

        public Phrases()
        {
            InitializeComponent();

            if (!Directory.Exists(App.StartupPath + "/Properties"))
            {
                _ = Directory.CreateDirectory(App.StartupPath + "/Properties");
            }

            if (!File.Exists(_profilePath))
            {
                File.Copy(App.StartupPath + "/Properties/BlankPhraseLayout.xml",_profilePath);
            } 

            this.LoadFromXml(_profilePath);
             
            _backItem = new ChildTalkerBackButton("Back",App.StartupPath + "/Resources/back.png", this, false);
            Scan.GoBackHold += GoBackHold_DeleteItem;
            _deletionTimer.Elapsed += DeletionTimerElapsed;
        }

        //required for all TalkerView  classes
        public override List<DependencyObject> GetParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                Controls,
                items
            };
            return parents;
        }

        /// <summary>
        /// Getter method for ViewParents
        /// </summary>
        /// <returns> a list of all Previous folder paths</returns>
        public Stack<ChildTalkerFolder> GetViewParents()
        {
            return ViewParents;
        }

        public void LoadFromXml(string path)
        {
            _profilePath = path;
            ViewParents = new Stack<ChildTalkerFolder>();
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlReader reader = XmlReader.Create(path))
            {
                _xmlWrapper = (ChildTalkerXmlWrapper)serializer.Deserialize(reader);
            }

            List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
            foreach (var child in _xmlWrapper.Children)
            {
                ctTiles.Add(ParseNode(child));
            }

            LoadTiles(ctTiles, true);
        }

        public void SaveToXml(string path)
        {
            var serializerSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlWriter writer = XmlWriter.Create(path, serializerSettings))
            {
                serializer.Serialize(writer, _xmlWrapper);
            }
        }

        public IChildTalkerTile ParseNode(ChildTalkerXml node)
        {
            if (node.TileType == ChildTalkerXml.Tile.talker)
            {
                return new ChildTalkerTile(node.Text, node.ImagePath, node.InColor);
            }
            else
            {
                List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
                foreach (var child in node.Children)
                {
                    ctTiles.Add(ParseNode(child));
                }
                ChildTalkerFolder folder = new ChildTalkerFolder(node.Text, node.ImagePath, this, !node.InColor);
                folder.SetChildren(ctTiles);
                return folder;
            }
        }

        /// <summary>
        /// when running the gobackHold event is occuring
        /// when timer elapses a popup appears asking if you would like to delete the highlighted element
        /// </summary>
        private readonly Timer _deletionTimer = new Timer(Scan.ScanTimerInterval * 2);
        /// <summary>
        /// Occurs when goBack is initially held down
        /// </summary>
        /// <param name="currentObj"></param>
        /// <param name="goBackDefaultEvent"></param>
        private void GoBackHold_DeleteItem(DependencyObject currentObj, Autoscan2.DefaultEvents goBackDefaultEvent)
        {
            if (currentObj is PhraseButton item)
            {
                Scan.PauseScan(true);
                Scan.GoBackPress += (curObj, gbd) => _deletionTimer.Stop();
                _deletionTimer.Elapsed += (s, e) => { this.Dispatcher.Invoke(() => { item.IsSelected = true; }); };
                _deletionTimer.Start();

            }
        }

        private void GoBackRelease_DeleteItem(DependencyObject currentObj, Autoscan2.DefaultEvents goBackDefaultEvent)
        {
            if (!(currentObj is PhraseButton item)) return;
            Scan.IgnoreGoBackPressOnce = true;
            Scan.PauseScan(false);
            if (item.ModifyThis())
                Scan.NewListToScanThough<PhraseButton>(items);
            else
                item.IsSelected = false;
            Scan.GoBackPress -= GoBackRelease_DeleteItem;
        }

        private void DeletionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Scan.GoBackPress += GoBackRelease_DeleteItem;
            Scan.IgnoreGoBackPressOnce = true;
            _deletionTimer.Stop();
        }

        public void LoadTiles(List<IChildTalkerTile> ctTiles, Boolean calledFromLoad = false)
        {
            
            //only true when page is first opened
            if (calledFromLoad)
            {
                _rootChildren = ctTiles;
            }
            else
            {
                if (!ctTiles.Contains(_backItem) && ViewParents.Count > 0)
                {
                    ctTiles.Insert(0, _backItem);
                    ViewParents.Peek().SetChildren(ctTiles);

                }
            }

            items.Children.Clear();
            foreach (IChildTalkerTile item in ctTiles)
            {
                PhraseButton ui = new PhraseButton();
                ui.SetItem(item);
                ui.SetParent(this);
                _ = items.Children.Add(ui);
            }

            if (calledFromLoad)
            {
                _xmlWrapper.Children = new List<ChildTalkerXml>();
                foreach (IChildTalkerTile rootChild in ctTiles)
                {
                    _xmlWrapper.Children.Add(rootChild.Xml);
                }
            }
            else
            { 
                Scan.NewListToScanThough<PhraseButton>(items, skipnextscan:true);
            }
        }

        public void AddSingleItem(IChildTalkerTile ctTileToAdd)
        {
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().AddChild(ctTileToAdd);
            }
            else
            {
                _rootChildren.Add(ctTileToAdd);
                _xmlWrapper.Children.Add(ctTileToAdd.Xml);
            }
            PhraseButton ui = new PhraseButton();
            ui.SetItem(ctTileToAdd);
            ui.SetParent(this);
            _ = items.Children.Add(ui);

            SaveToXml(_profilePath);
        }

        public void RemoveSingleTile(PhraseButton itemToRemove)
        {
            items.Children.Remove(itemToRemove);
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().RemoveChild(itemToRemove.CtTile);
            }
            else
            {
                _ = _rootChildren.Remove(itemToRemove.CtTile);
                _ = _xmlWrapper.Children.Remove(itemToRemove.CtTile.Xml);
            }
            SaveToXml(_profilePath);
        }

        public void UpdateSavedTiles(PhraseButton itemToChange, String path)
        {
            itemToChange.CtTile.Xml.ImagePath = path;
            SaveToXml(_profilePath);
        }

        public void PopFolderView()
        {
            if (ViewParents.Count < 2)
            {
                _ = ViewParents.Pop();
                LoadTiles(_rootChildren);
            }
            else
            {
                _ = ViewParents.Pop();
                List<IChildTalkerTile> children = ViewParents.Peek().Children;
                LoadTiles(children);
            }
        }


        /// <summary>
        /// Creates a folder with the provided name. If the name is empty then the folder will not be created
        /// </summary>
        /// <param name="folderName">Name given to the desired folder</param>
        private void CreateFolder(string folderName)
        {
            if (folderName != "")
            {
                ChildTalkerFolder ctFolder = new ChildTalkerFolder(folderName, App.StartupPath + "/Resources/folder.jpg", this, false);
                AddSingleItem(ctFolder);
            }
        }

        /// <summary>
        /// Action listener that will begin the work flow to start creating a folder tile. This will bring up a keyboard for
        /// the user to name their folder. Once that keyboard closes, the typed string will be returned and passed into the
        /// CreateFolder method
        /// </summary>
        /// <param name="sender">Button used to initiate this process</param>
        /// <param name="e"></param>
        private void InsertFolderTile_Click(object sender, RoutedEventArgs e)
        {
            KeyboardPopup board = new KeyboardPopup();
            string folderName = board.GetUserInput();
            CreateFolder(folderName);
        }



        /// <summary>
        /// Action Listener that will begin the work flow to start creating a folder tile. This will bring up a keyboard for the user
        /// to name the tile they want to create. Once that keyboard closes, an ImageGenerator will popup that will allow the user
        /// to select an image for the tile. Will then make use of the CreateTile method once that data has been collected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertTalkerTile_Click(object sender, RoutedEventArgs e)
        {
            KeyboardPopup board = new KeyboardPopup();
            string tileName = board.GetUserInput();

            if (tileName != "")
            {
                string path = ImageGenerator.ImagePopup();
                CreateTile(path, tileName);
            }
        }


        /// <summary>
        /// Creates TalkerTile with the image and phrase that are passed in. It will then add it to the current page
        /// </summary>
        /// <param name="imagePath">Path for the image being added to the tile</param>
        /// <param name="phrase">Phrase that will be spoken when the tile is clicked</param>
        private void CreateTile(string imagePath, string phrase)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
                ChildTalkerTile ctItem = new ChildTalkerTile(phrase, imagePath, false);
                AddSingleItem(ctItem);
        }

        

    }  
}
