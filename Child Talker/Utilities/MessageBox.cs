/* Todo provide proper copyright information for this
 * Message Box is based on the work of evanwon
 * see github.com evanwon WPFCustomMessageBox
 */

using System.Windows;

namespace Child_Talker.Utilities
{ 
    /// <summary>
    /// Displays a message box.
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// Displays a message box that has a message and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult Show(string messageBoxText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText);
            msg.ShowDialog();

            return msg.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, FrameworkElement messageBoxContent)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, messageBoxContent);
            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message and returns a result.
        /// </summary>
        /// <param name="owner">A System.Windows.Window that represents the owner window of the message box.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText);
            msg.Owner = owner;
            msg.ShowDialog();

            return msg.Result;
        }
        
        /// <summary>
        /// Displays a message box that has a message, title bar caption, and button; and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A System.Windows.MessageBoxButton value that specifies which button or buttons to display.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, button);
            msg.ShowDialog();

            return msg.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, FrameworkElement messageBoxContent, MessageBoxButton button)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, messageBoxContent, button);
            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, and OK button with a custom System.String value for the button's text; and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>FrameworkElement messageVisual
        /// <param name="okButtonText">A System.String that specifies the text to display within the OK button.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult ShowOK(string messageBoxText, string okButtonText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, MessageBoxButton.OK);
            msg.OkButtonText = okButtonText;

            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Displays a message box that has a message, caption, and OK/Cancel buttons with custom System.String values for the buttons' text;
        /// and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="okButtonText">A System.String that specifies the text to display within the OK button.</param>
        /// <param name="cancelButtonText">A System.String that specifies the text to display within the Cancel button.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult ShowOKCancel(string messageBoxText, string okButtonText, string cancelButtonText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, MessageBoxButton.OKCancel);
            msg.OkButtonText = okButtonText;
            msg.CancelButtonText = cancelButtonText;

            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Displays a message box that has a message, caption, and Yes/No buttons with custom System.String values for the buttons' text;
        /// and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="yesButtonText">A System.String that specifies the text to display within the Yes button.</param>
        /// <param name="noButtonText">A System.String that specifies the text to display within the No button.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult ShowYesNo(string messageBoxText, string yesButtonText, string noButtonText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, MessageBoxButton.YesNo);
            msg.YesButtonText = yesButtonText;
            msg.NoButtonText = noButtonText;

            msg.ShowDialog();

            return msg.Result;
        }

        /// <summary>
        /// Displays a message box that has a message, caption, and Yes/No/Cancel buttons with custom System.String values for the buttons' text;
        /// and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="yesButtonText">A System.String that specifies the text to display within the Yes button.</param>
        /// <param name="noButtonText">A System.String that specifies the text to display within the No button.</param>
        /// <param name="cancelButtonText">A System.String that specifies the text to display within the Cancel button.</param>
        /// <returns>A System.Windows.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static MessageBoxResult ShowYesNoCancel(string messageBoxText, string yesButtonText, string noButtonText, string cancelButtonText)
        {
            MessageBoxWindow msg = new MessageBoxWindow(messageBoxText, MessageBoxButton.YesNoCancel);
            msg.YesButtonText = yesButtonText;
            msg.NoButtonText = noButtonText; 
            msg.CancelButtonText = cancelButtonText;

            msg.ShowDialog();
            return msg.Result;
        }
    }
}
