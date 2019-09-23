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
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : TalkerView
    {      
        private readonly Autoscan2 scan = Autoscan2.Instance;
       
        public MediaPlayer()
        {
            InitializeComponent();        
        }
    }
}
