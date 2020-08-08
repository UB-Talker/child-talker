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
using System.Xml;
using Child_Talker.Utilities;
using RokuCommands;
using Child_Talker.TalkerButton;
using Child_Talker.Utilities.Autoscan;
using Button = Child_Talker.TalkerButton.Button;

namespace Child_Talker.TalkerViews.TVandRoku
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Remote : TalkerView
    {
        public Remote()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public override List<DependencyObject> GetParents()
        {
            List<DependencyObject> parents = new List<DependencyObject>()
                {
                    LeftColumn,
                    Options,
                    Arrows,
                    Media,
                };
            return (parents);
            
        }

        private void sendInput(object sender, RoutedEventArgs e)
        {
            var btn = sender as TalkerButton.Button;
            var tag = (string)(btn.Tag);
            _ = RokuHttp.PressKey(tag);
        }

        private async Task Launcher_OnClickAsync(object sender, RoutedEventArgs e)
        {
            var apps = await RokuHttp.GetAllChannels();
            ScrollingSelectionWindow ssw = new ScrollingSelectionWindow();
            foreach (var app in apps.OrderBy(a => a.Name))
            {
                await ssw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Button btn = new Button()
                    {
                        Text = app.Name,
                        Tag = app.Id
                    };
                    ssw.addElement(btn);
                    //Autoscan2.Instance.GoBackPress += (s,_) => { ssw.Close(); };
                    btn.Click += (s, _) =>
                    {
                        ssw.result = (string)btn.Tag;
                        ssw.Close();
                    };
                }));
                Console.WriteLine("{0}.\t {1}",app.Id,app.Name);
            }
            string result  = (string)ssw.prompt();
            if (result is null) return;
            Console.WriteLine("selected result");
            Console.WriteLine(result);
            _ = RokuHttp.LaunchChannel(result);














        }

        private void Launcher_OnClick(object sender, RoutedEventArgs e)
        {
            _ = Launcher_OnClickAsync(sender, e);
        }
    }
}
