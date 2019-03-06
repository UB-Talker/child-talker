using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for WindowHistory.xaml
    /// </summary>
    public partial class WindowHistory : TalkerView
    {
        private SpeechSynthesizer synth;
        private String selectedText;
        private Button selectedButton;


        public WindowHistory()
        {
            InitializeComponent();
            
            synth = new SpeechSynthesizer();
            selectedText = "";
        }


        public void openKeyboardWithText(object sender, RoutedEventArgs args)
        {
            Window.GetWindow(this).DataContext = new Keyboard(selectedText);
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
        }


        /*
         * Adds functionality to the SPEAK Button. The synth instance variable speaks whatever is stored in the 
         * selectedText instance variable.
         */
        public void speakSelectedText(object sender, RoutedEventArgs args){
            synth.Speak(selectedText);
        }
    }
}
