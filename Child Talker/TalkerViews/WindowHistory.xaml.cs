using Child_Talker.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for WindowHistory.xaml
    /// </summary>
    public partial class WindowHistory : TalkerView
    {
        private  readonly TextUtility util; //Used to speak the currently selected text
        private string selectedText; //Is set when the user chooses one of the phrases in the menu
        private TlkrBTN selectedButton; //The currently pressed button
        private readonly Autoscan scan;
        public WindowHistory()
        {
            InitializeComponent();        
            scan = Autoscan.Instance; //singleton cannot call constructor, call instance
            util = TextUtility.Instance;
            selectedText = "";
            addPhrases();
            phraseStack.ScrollOwner = scrollViewer;
            scrollViewer.ScrollToEnd();
        }
        
        /*
         * If there is text selected, the keyboard will open up with the text in its TextBlock
         */
        public void openKeyboardWithText(object sender, RoutedEventArgs args)
        {
            getWindow().changeView(new Keyboard(selectedText));
        }

        /*
         * When one of the items in the scroll pane are selected, the selectedText instance/member variable will assume
         * the value of the string contained in the TextBlock that is the child of the 'sender' Button.
         * If the currently selected button is clicked again it clears the selectedText variable
         */
        public void selectText(object sender, RoutedEventArgs args){
            TlkrBTN button = sender as TlkrBTN;
            TextBlock textBlock = button.Content as TextBlock;
            if(selectedButton == default(Button)){
                selectedButton = button;
                selectedText = textBlock.Text;
                button.Background = Brushes.DarkGray;
            } else if(button == selectedButton){
                button.Background = Brushes.Black;
                selectedText = "";
                selectedButton = null;
            } else{
                selectedButton.Background = Brushes.Black;
                selectedText = textBlock.Text;
                button.Background = Brushes.DarkGray;
                selectedButton = button;
            }

            if(getWindow().IsScanning()) //stops autoscan so selected text can be used
            {
                scan.StartAutoscan<Button>(sidePanel);
            }
        }


        /*
         * Adds functionality to the SPEAK Button. The synth instance variable speaks whatever is stored in the 
         * selectedText instance variable.
         */
        public void speakSelectedText(object sender, RoutedEventArgs args){
            util.speak(selectedText);
            update();
        }


        /*
         * Adds the stored list of historically spoken phrases to the GUI as Buttons
         */
        private void addPhrases()
        {
            foreach (Tuple<DateTime, string> pair in util.getSpokenPhrases())
            {
                TextBlock phrase = new TextBlock { Text = pair.Item2 };

                TlkrBTN phraseButton = new TlkrBTN
                {
                    Background = Brushes.Black,
                    Content = phrase
                };
                phraseButton.Click += selectText;
                phraseStack.Children.Add(phraseButton);

            }
        }
        
        /*
         * This is used if the user navigates "Back" to this page. The list of buttons
         * on the GUI must be refreshed if the user were to speak new phrases.
         * 
         * *** EFFICIENCY ISSUES ***
         * This could be made more efficient. Some ideas:
         * 
         * 1). Maintain a list of phrases that were spoken while the application was running
         *     and add those to the GUI when needed instead of deleting and reading all of the
         *     Butttons.
         */
        public override void update()
        {
            phraseStack.Children.Clear();
            addPhrases();
        }

        /* Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         * Currently goes between the phrase stack and side menu
         */
        public override List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
            {
                phraseStack,
                sidePanel
            };
            return (parents);
        }
    }
}
