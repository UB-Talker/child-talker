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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Child_Talker.TalkerViews.Keyboard
{
    /// <summary>
    /// Interaction logic for KeyboardPopup.xaml
    /// </summary>
    public partial class KeyboardPopup : SecondaryWindow
    {
        /// <summary>
        /// The default value for the number of characters allowed in the Keyboard
        /// </summary>
        public static readonly int DEFAULT_LENGTH = 25;

        /// <summary>
        /// Allows for an unlimited number of characters in the keyboard's textbox at once
        /// </summary>
        public static readonly int UNLIMITED_LENGTH = 0;

        /// <summary>
        /// Will create a KeyboardPopup object with the textbox size set to the default length
        /// </summary>
        public KeyboardPopup()
        {
            InitializeComponent();
            prepareKeyboard(DEFAULT_LENGTH, "");
            
            Margin = new Thickness(130, 0, 130, 0);
        }


        /// <summary>
        /// Allows for a custom character limit to be chosen for the textbox.
        /// </summary>
        /// <param name="characterLimit">Inputing a negative number will result in it being 
        /// set to zero. </param>
        public KeyboardPopup(int characterLimit)
        {
            InitializeComponent();
            prepareKeyboard(characterLimit, "");
        }

        /// <summary>
        /// This will halt the execution of the window that opened this popup until this method returns.
        /// Override of the inherited Show(...) methods from the SecondaryWindow class
        /// </summary>
        /// <returns>Returns the string that was typed into the keyboard</returns>
        public string GetUserInput()
        {
            // Calls the show method defined in the SecondaryWindow class
            Show(new List<DependencyObject>() { keyboard.keyboardGrid, keyboard.autofill });

            //ShowDialog();
            return keyboard.textBox.Text;
        }

        private void prepareKeyboard(int characterLimit, string text)
        {
            if(characterLimit < 0)
            {
                characterLimit = 0;
            }
            keyboard.AddTextBox();
            keyboard.textBox.CharacterCasing = CharacterCasing.Lower;
            keyboard.textBox.MaxLength = characterLimit;
            keyboard.textBox.Text += text;
            keyboard.defaultEnterPress = false;
            keyboard.EnterPress += (senderK, eK) => { this.Close(); };

        }

        
    }
}
