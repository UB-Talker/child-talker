using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Child_Talker.TalkerButton
{
    public partial class Button : System.Windows.Controls.Button
    {
        public static readonly ControlTemplate VerticalTemplate = Application.Current.Resources["VerticalImage"] as ControlTemplate;
        public static readonly ControlTemplate HorizontalTemplate = Application.Current.Resources["HorizontalImage"] as ControlTemplate;
        public static readonly ControlTemplate TextOnlyTemplate = Application.Current.Resources["TextOnly"] as ControlTemplate;

        /// <summary>
        /// 
        /// </summary>
        public bool Selected = false;


        public Button()
        {
            this.DataContextChanged += OnContentChanged;
        }

        public void OnContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(((Button)sender).Text) )
            {
                if (((Button) sender).Content is string s)
                {
                 //   ((Button) sender).Text = s;
                }

            }
        }


    }

}
