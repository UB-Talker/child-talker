using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Child_Talker.Utilities;


namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for WindowHistory.xaml
    /// </summary>
    public partial class WindowHistory : TalkerView
    {
        private TextUtility util; //Used to speak the currently selected text
        private string selectedText; //Is set when the user chooses one of the phrases in the menu
        private Button selectedButton; //The currently pressed button

        public WindowHistory()
        {
            InitializeComponent();        
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
            Button button = sender as Button;
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

            if(getWindow().isScanning()) //stops autoscan so selected text can be used
            {
                Autoscan sc = getWindow().toggleAutoscan(); //stops autoscan
                sc.partialAutoscan<DependencyObject>(sidePanel,getWindow()); //partial scans side panel
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
                Button phraseButton = new Button();
                TextBlock phrase = new TextBlock();
                phrase.Text = pair.Item2;

                phraseButton.Background = Brushes.Black;
                phraseButton.Click += selectText;
                phraseButton.Content = phrase;

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
         *     and add those to the GUI when needed instead of deleting and readding all of the
         *     Butttons.
         */
        override public void update()
        {
            phraseStack.Children.Clear();
            addPhrases();
        }

        /* Used for autoscan, please update if xaml is changed
         * Must return the panels to iterate through when autoscan is first initialized on this page
         * Currently goes between the phrase stack and side menu
         */
        override public List<DependencyObject> getParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>();
            parents.Add(phraseStack);
            parents.Add(sidePanel);
            return (parents);
        }
    }
}
