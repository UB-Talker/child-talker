using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SysButton = System.Windows.Controls.Button;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Child_Talker.Utilities.Autoscan;
using Button = Child_Talker.TalkerButton.Button;

namespace Child_Talker.Utilities
{
    internal partial class MessageBoxWindow : SecondaryWindow
    {
        internal string Caption
        {
            get => Title;
            set => Title = value;
        }
        internal string Message
        {
            get => TextBlock_Message.Text;
            set => TextBlock_Message.Text = value;
        }
        internal string OkButtonText
        {
            get => Button_OK.Text;
            set => Button_OK.Text = value;
        }
        internal string CancelButtonText
        {
            get => Button_Cancel.Text;
            set => Button_Cancel.Text = value;
        }
        internal string YesButtonText
        {
            get => Button_Yes.Text;
            set => Button_Yes.Text = value;
        }
        internal string NoButtonText
        {
            get => Button_No.Text;
            set => Button_No.Text = value;
        }
        public MessageBoxResult Result { get; set; }

        internal MessageBoxWindow(string message)
        {
            InitializeComponent();

            Message = message;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal MessageBoxWindow(string message, MessageBoxButton button)
        {
            InitializeComponent();

            Message = message;
            DisplayButtons(button);
        }

        internal MessageBoxWindow(string message, FrameworkElement messageBoxContent)
        {
            InitializeComponent();

            Message = message;
            Content_MessageBox.Content = messageBoxContent;
            Content_MessageBox.Visibility = Visibility.Visible;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal MessageBoxWindow(string message, FrameworkElement messageBoxContent, MessageBoxButton button)
        {
            InitializeComponent();

            Message = message;
            Content_MessageBox.Content = messageBoxContent;
            Content_MessageBox.Visibility = Visibility.Visible;
            DisplayButtons(button);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    // Hide all but OK, Cancel
                    Button_OK.Visibility = System.Windows.Visibility.Visible;
                    Button_OK.Focus();
                    Button_Cancel.Visibility = System.Windows.Visibility.Visible;

                    Button_Yes.Visibility = System.Windows.Visibility.Collapsed;
                    Button_No.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    // Hide all but Yes, No
                    Button_Yes.Visibility = System.Windows.Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = System.Windows.Visibility.Visible;

                    Button_OK.Visibility = System.Windows.Visibility.Collapsed;
                    Button_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    // Hide only OK
                    Button_Yes.Visibility = System.Windows.Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = System.Windows.Visibility.Visible;
                    Button_Cancel.Visibility = System.Windows.Visibility.Visible;

                    Button_OK.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    // Hide all but OK
                    Button_OK.Visibility = System.Windows.Visibility.Visible;
                    Button_OK.Focus();

                    Button_Yes.Visibility = System.Windows.Visibility.Collapsed;
                    Button_No.Visibility = System.Windows.Visibility.Collapsed;
                    Button_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
            Autoscan2.Instance.NewListToScanThough<Button>(ButtonDock);
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }
        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }
        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }
    }
}
