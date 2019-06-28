using Child_Talker.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Child_Talker.TalkerButton;
using Button = Child_Talker.TalkerButton.Button;

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for WindowHistory.xaml
    /// </summary>
    public partial class WindowHistory : TalkerView
    {
        private  readonly TextUtility util; //Used to speak the currently selected text
        private string selectedText; //Is set when the user chooses one of the phrases in the menu
        private Button selectedButton; //The currently pressed button
        private readonly Autoscan2 scan;
        public WindowHistory()
        {
            InitializeComponent();        
            scan = Autoscan2.Instance; //singleton cannot call constructor, call instance
            util = TextUtility.Instance;
            selectedText = "";
            addPhrases();
            phraseStack.ScrollOwner = scrollViewer;
            scrollViewer.ScrollToEnd();
        }
        
        /// <summary>
        /// If there is text selected, the keyboard will open up with the text in its TextBlock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OpenKeyboardWithText(object sender, RoutedEventArgs args)
        {
            MainWindow.Instance.ChangeView(new Keyboard(selectedText));
        }

        /// <summary>
        /// When one of the items in the scroll pane are selected, the selectedText instance/member variable will assume
        /// the value of the string contained in the TextBlock that is the child of the 'sender' Button.
        /// If the currently selected button is clicked again it clears the selectedText variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SelectText(object sender, RoutedEventArgs args){
            Button button = sender as Button;
            if (button.Selected == false)
            {
                button.Selected = true;
                selectedButton = button;
                selectedText = (button.Text);
                //need to select element
                //Autoscan2.HighlightedElementInfo.HighlightSelection(button);

                scan.NewListToScanThough<System.Windows.Controls.Button>(sidePanel);
            }
            else
            {
                //button.ResetTkrForeground();
                selectedText = "";
                selectedButton = null;
            }
        }


         /// <summary>
         /// Adds functionality to the SPEAK Button. The synth instance variable speaks whatever is stored in the 
         /// selectedText instance variable.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="args"></param>
        public void speakSelectedText(object sender, RoutedEventArgs args){
            util.Speak(selectedText);
            //selectedButton?.ResetTkrForeground();
            Update();
        }

        /// <summary>
        /// Adds the stored list of historically spoken phrases to the GUI as Buttons
        /// </summary>
        private void addPhrases()
        {
            foreach (Tuple<DateTime, string> pair in util.getSpokenPhrases())
            {
                Button phraseButton = new Button
                {
                    Text = pair.Item2,
                    Height = 140
                };
                phraseButton.Click += SelectText;
                phraseStack.Children.Add(phraseButton);
            }
        }
        
        /// <remarks>
        /// update()
        /// *** EFFICIENCY ISSUES ***
        /// This could be made more efficient. Some ideas:
        /// 
        /// 1). Maintain a list of phrases that were spoken while the application was running
        ///     and add those to the GUI when needed instead of deleting and reading all of the
        ///     Buttons.
        /// </remarks>
        
        /// <summary>
        /// This is used if the user navigates "Back" to this page. The list of buttons
        /// on the GUI must be refreshed if the user were to speak new phrases.
        /// </summary>
        public override void Update()
        {
            phraseStack.Children.Clear();
            addPhrases();
        }

        /// <summary>
        /// Used for autoscan, please update if xaml is changed
        /// Must return the panels to iterate through when autoscan is first initialized on this page
        /// Currently goes between the phrase stack and side menu
        /// </summary>
        /// <returns></returns>
        public override List<DependencyObject> GetParents()
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
