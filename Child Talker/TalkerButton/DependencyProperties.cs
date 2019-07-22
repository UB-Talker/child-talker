using System;
using System.Windows;
using System.Windows.Controls;

namespace Child_Talker.TalkerButton
{
    //Dependency Properties relating to Autoscan Functionality
    public partial class Button : System.Windows.Controls.Button
    {
        /// <summary>
        /// when this Button is selected the autoscan stops cycling until 'go back' is pressed
        /// <para>   true - pause autoscan</para>
        /// <para>   false - autoscan continues as normal     (default)</para>
        /// </summary>
        public static readonly DependencyProperty PauseOnSelectProperty = DependencyProperty.Register(
            "PauseOnSelect", typeof(bool), typeof(Button), new UIPropertyMetadata(false));

        /// <summary>
        /// when this Button is selected the autoscan stops cycling until 'go back' is pressed
        /// <para>   true - pause autoscan</para>
        /// <para>   false - autoscan continues as normal     (default)</para>
        /// </summary>
        public bool PauseOnSelect {
            get => (bool)GetValue(PauseOnSelectProperty);
            set => SetValue(PauseOnSelectProperty, value); }
    }

    //Dependency Properties Relating To Button Appearance
    public partial class Button : System.Windows.Controls.Button
    {

        [Flags]
        public enum LayoutEnum { None=0, TextOnly =1, ImageOnly =2, Vertical =3, Horizontal =7, Override = 8 };
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
            "Layout", typeof(LayoutEnum), typeof(Button), new UIPropertyMetadata(LayoutEnum.TextOnly));
        public LayoutEnum Layout 
        {
            get => (LayoutEnum) GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }


        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(string), typeof(Button), new UIPropertyMetadata("",ImageSourceChanged));
        public string ImageSource
        {
            get => (string) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        private static void ImageSourceChanged(DependencyObject d , DependencyPropertyChangedEventArgs e) 
        {
            if (!(d is Button btn)) return;
            string s = (string)e.NewValue;
            btn.ImageSource = e.NewValue as string;
            if(btn.Layout == LayoutEnum.Override || btn.Layout == LayoutEnum.None) return;
            if (string.IsNullOrEmpty(btn.ImageSource))
                btn.Layout = LayoutEnum.TextOnly;
            else
                btn.Layout |= LayoutEnum.ImageOnly;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(Button), new UIPropertyMetadata("", TextChanged));

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Button btn)) return;
            btn.Text = e.NewValue as string;
            if(btn.Layout == LayoutEnum.Override || btn.Layout == LayoutEnum.None) return;
            if (string.IsNullOrEmpty(btn.Text))
                btn.Layout = LayoutEnum.ImageOnly;
            else
                btn.Layout |= LayoutEnum.TextOnly;
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(Button), new UIPropertyMetadata(false));
        public bool IsSelected 
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// When true the "ImageSource" is displayed in its original colors. When false a silhouette matching system foreground color is displayed
        /// </summary>
        public static readonly DependencyProperty InColorProperty = DependencyProperty.Register(
            "InColor", typeof(bool), typeof(Button), new UIPropertyMetadata(false));
        /// <summary>
        /// When true the "ImageSource" is displayed in its original colors. When false a silhouette matching system foreground color is displayed.
        /// Has no effect if layout is set to None, Override, TextOnly
        /// </summary>
        public bool InColor 
        {
            get => (bool) GetValue(InColorProperty);
            set => SetValue(InColorProperty, value);
        }

    }
}
