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

namespace Child_Talker.TalkerViews
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : TalkerView
    {
        private string text;

        public UserControl1()
        {
            InitializeComponent();
        }


        public void type(object sender, RoutedEventArgs args)
        {
            Button button = (Button)sender;
            string key = button.ContentStringFormat;

            if(key == "SPACE")
            {
                text += " ";
            } else if(key == "Backspace" && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
            } else
            {
                text += key;
            }

            _Text.Text = text;
            
        }

    }
}
