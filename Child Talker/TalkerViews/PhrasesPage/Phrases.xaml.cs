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
using Timer = System.Timers.Timer;

namespace Child_Talker.TalkerViews.PhrasesPage
{
    /// <summary>
    /// Interaction logic for PhrasesPage.xaml
    /// </summary>
    public partial class Phrases : TalkerView
    {
        private ChildTalkerXmlWrapper XmlWrapper;
        private IChildTalkerTile backItem;
        private string ProfilePath;
        private List<IChildTalkerTile> rootChildren = new List<IChildTalkerTile>();
        public Stack<ChildTalkerFolder> ViewParents = new Stack<ChildTalkerFolder>();

        private static readonly Autoscan2 scan = Autoscan2.Instance;

        public Phrases()
        {
            InitializeComponent();
            ProfilePath = "../../Resources/example2.xml";
            backItem = new ChildTalkerBackButton("Back", "../../Resources/back.png", this, false);
            if (!File.Exists(ProfilePath))
                File.Create(ProfilePath);
            else
                this.LoadFromXml(ProfilePath);
            scan.GoBackHold += GoBackHold_DeleteItem;
            deletionTimer.Elapsed += DeletionTimerElapsed;
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

        public void LoadFromXml(string _path)
        {
            ProfilePath = _path;
            ViewParents = new Stack<ChildTalkerFolder>();
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlReader reader = XmlReader.Create(_path))
            {
                XmlWrapper = (ChildTalkerXmlWrapper) serializer.Deserialize(reader);
            }

            List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
            foreach (var child in XmlWrapper.Children)
            {
                ctTiles.Add(ParseNode(child));
            }

            LoadTiles(ctTiles, true);
        }

        public void SaveToXml(string _path)
        {
            var serializerSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { typeof(ChildTalkerXmlWrapper) })[0];
            using (XmlWriter writer = XmlWriter.Create(_path, serializerSettings))
            {
                serializer.Serialize(writer, XmlWrapper);
            }
        }

        public IChildTalkerTile ParseNode(ChildTalkerXml _node)
        {
            if (_node.TileType == ChildTalkerXml.Tile.talker)
            {
                return new ChildTalkerTile(_node.Text, _node.ImagePath, _node.InColor);
            } else
            {
                List<IChildTalkerTile> ctTiles = new List<IChildTalkerTile>();
                foreach (var child in _node.Children)
                {
                    ctTiles.Add(ParseNode(child));
                }
                ChildTalkerFolder folder = new ChildTalkerFolder(_node.Text, _node.ImagePath, this, !_node.InColor);
                folder.SetChildren(ctTiles);
                return folder;
            }
        }

        /// <summary>
        /// when running the gobackHold event is occuring
        /// when timer elapses a popup appears asking if you would like to delete the highlighted element
        /// </summary>
        private readonly Timer deletionTimer = new Timer(scan.ScanTimerInterval*2);
        /// <summary>
        /// Occurs when goBack is initially held down
        /// </summary>
        /// <param name="currentObj"></param>
        /// <param name="goBackDefaultEvent"></param>
        private void GoBackHold_DeleteItem(DependencyObject currentObj, Autoscan2.DefaultEvents goBackDefaultEvent)
        {
            if (currentObj is PhraseButton item)
            {
                scan.PauseScan(true);
                scan.GoBackPress += (curObj, gbd) => deletionTimer.Stop();
                deletionTimer.Elapsed += (s, e) => { this.Dispatcher.Invoke(() => { item.IsSelected = true; }); };
                deletionTimer.Start();

            }
        }

        private void GoBackRelease_DeleteItem(DependencyObject currentObj, Autoscan2.DefaultEvents goBackDefaultEvent)
        {
            if (!(currentObj is PhraseButton item)) return;
            scan.IgnoreGoBackPressOnce = true;
            scan.PauseScan(false);
            if (item.DeleteThis())
                scan.NewListToScanThough<PhraseButton>(items);
            else
                item.IsSelected = false;
            scan.GoBackPress -= GoBackRelease_DeleteItem;
        }

        private void DeletionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            scan.GoBackPress += GoBackRelease_DeleteItem;
            scan.IgnoreGoBackPressOnce = true;
            deletionTimer.Stop();
        }

        public void LoadTiles(List<IChildTalkerTile> _ctTiles, Boolean calledFromLoad = false)
        {
            //only true when page is first opened
            if (calledFromLoad) 
            {
                rootChildren = _ctTiles;
            }
            else
            {
                if(!_ctTiles.Contains(backItem) && ViewParents.Count > 0)
                {
                    _ctTiles.Insert(0, backItem);
                    ViewParents.Peek().SetChildren(_ctTiles);
                }
            }

            items.Children.Clear();
            foreach (IChildTalkerTile item in _ctTiles)
            {
                PhraseButton ui = new PhraseButton();
                ui.SetItem(item);
                ui.SetParent(this);
                items.Children.Add(ui);
            }

            if (calledFromLoad)
            {
                XmlWrapper.Children = new List<ChildTalkerXml>();
                foreach (IChildTalkerTile rootChild in _ctTiles)
                {
                    XmlWrapper.Children.Add(rootChild.Xml);
                }
            }
        }

        public void AddSingleItem(IChildTalkerTile _ctTileToAdd)
        {
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().AddChild(_ctTileToAdd);
            }
            else
            {
                rootChildren.Add(_ctTileToAdd);
                XmlWrapper.Children.Add(_ctTileToAdd.Xml);
            }
            PhraseButton ui = new PhraseButton();
            ui.SetItem(_ctTileToAdd);
            ui.SetParent(this);
            items.Children.Add(ui);

            SaveToXml(ProfilePath);
        }

        public void RemoveSingleTile(PhraseButton _itemToRemove)
        {
            items.Children.Remove(_itemToRemove);
            if (ViewParents.Count > 0)
            {
                ViewParents.Peek().RemoveChild(_itemToRemove.CtTile);
            }
            else
            {
                rootChildren.Remove(_itemToRemove.CtTile);
                XmlWrapper.Children.Remove(_itemToRemove.CtTile.Xml);
            }
            SaveToXml(ProfilePath);
        }

        public void PopFolderView()
        {
            if (ViewParents.Count < 2)
            {
                ViewParents.Pop();
                LoadTiles(rootChildren);
            }
            else
            {
                ViewParents.Pop();
                List<IChildTalkerTile> children = ViewParents.Peek().Children;
                LoadTiles(children);
            }
        }

        private SecondaryWindow popupWindow;
        private KeyboardLayout keyboard;
        public void CreateFolder(object sender, RoutedEventArgs e)
        {
            String inputPhrase = keyboard.textBox.Text;
            if (inputPhrase != "")
            {
                ChildTalkerFolder ctFolder = new ChildTalkerFolder(inputPhrase, "../../Resources/folder.jpg", this, false);
                AddSingleItem(ctFolder);
            }
            popupWindow.Close();
        }

        private void InsertFolderTile_Click(object sender, RoutedEventArgs e)
        {
            NewKeyboardPopup();
            keyboard.defaultEnterPress = false;
            keyboard.EnterPress += CreateFolder;
            popupWindow.Show(new List<DependencyObject>(){keyboard.keyboardGrid, keyboard.autofill});
        }

        private void NewKeyboardPopup()
        {
            keyboard = new KeyboardLayout();
            popupWindow = new SecondaryWindow(keyboard);
            keyboard.Margin=new Thickness(15);
            keyboard.AddTextBox();
            keyboard.textBox.CharacterCasing = CharacterCasing.Lower;
            keyboard.textBox.MaxLength = 25;
            //redefines size of secondaryWindow to fit keyboard
            popupWindow.Margin = new Thickness(130, 0, 130, 0);
            popupWindow.Content = keyboard;

            // invocations
            keyboard.BackPressWhenEmpty += ((sender, e) =>
            {
                popupWindow.Close(); (MainWindow.Instance).Show();
                MainWindow.Instance.Navigator.Refresh();
            });
            popupWindow.Show(new List<DependencyObject>(){keyboard.keyboardGrid, keyboard.autofill});
        }

        private void InsertTalkerTile_Click(object sender, RoutedEventArgs e)
        {
            NewKeyboardPopup();
            keyboard.defaultEnterPress = false;
            keyboard.EnterPress += (senderK, eK) => {CreateTile();};
        }

        private void OnIconSelect(string imagePath)
        {
            if (inputPhrase != "")
            {
                popupWindow.Close();
                MainWindow.Instance.Show();
                
                //String imagePath = PromptFileExplorer();
                if (imagePath != "")
                {
                    ChildTalkerTile ctItem = new ChildTalkerTile(inputPhrase, imagePath, false);
                    AddSingleItem(ctItem);
                }
            }

        }

        private string inputPhrase = "";
        private void CreateTile()
        {
            inputPhrase = keyboard.textBox.Text;
            popupWindow.Close();

            //popupWindow = new SecondaryWindow(MainWindow.Instance, this);
            ImageGenerator ig = new ImageGenerator(OnIconSelect);
            ig.Show();
            //popupWindow.DataContext = ig;
        }

    }
}
