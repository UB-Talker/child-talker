using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SysButton = System.Windows.Controls.Button;
using System.Windows.Media;

namespace Child_Talker.TalkerButton
{
    public partial class Button : SysButton
    {

        /// <summary>
        /// 
        /// </summary>
        public bool Selected = false;


        public Button()
        {
            this.DataContextChanged += OnContentChanged;
            //this casting is very important do not remove
        }


        public void OnContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Layout = LayoutEnum.None;
        }

    }

}
