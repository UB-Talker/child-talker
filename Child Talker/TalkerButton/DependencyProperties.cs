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


        // TODO make functional
        /// <summary>
        /// !!!!!!! NOT YET IMPLEMENTED !!!!!!!
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item </para>
        /// <para>   false - autoscan continues as normal     (default) </para>
        /// </summary>
        public static readonly DependencyProperty DoNotScanProperty = DependencyProperty.Register(
            "DoNotScan", typeof(bool), typeof(Button), new UIPropertyMetadata(null));
        // TODO make functional
        /// <summary>
        /// !!!!!!! NOT YET IMPLEMENTED !!!!!!!
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item </para>
        /// <para>   false - autoscan continues as normal     (default) </para>
        /// </summary>
        public bool DoNotScan
        {
            get => (bool)GetValue(DoNotScanProperty);
            set => SetValue(DoNotScanProperty, value);
        }

    }

    //Dependency Properties Relating To Button Appearance
    public partial class Button : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(string), typeof(Button), new UIPropertyMetadata("",ImageSourceDefined));
        public string ImageSource
        {
            get => (string) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        private static void ImageSourceDefined(DependencyObject d , DependencyPropertyChangedEventArgs e) 
        {
            if (!(d is Button btn)) return;
            btn.ImageSource = e.NewValue as string;
            if (string.IsNullOrEmpty(btn.ImageSource))
            {
                btn.Layout = LayoutEnum.TextOnly;
            }
            else if (btn.Layout == LayoutEnum.TextOnly)
            {
                btn.Layout = LayoutEnum.Vertical;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(Button), new UIPropertyMetadata(null));
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(Button), new UIPropertyMetadata(false));
        public bool IsHighlighted 
        {
            get => (bool) GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(Button), new UIPropertyMetadata(false));
        public bool IsSelected 
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty InColorProperty = DependencyProperty.Register(
            "InColor", typeof(bool), typeof(Button), new UIPropertyMetadata(false));
        public bool InColor 
        {
            get => (bool) GetValue(InColorProperty);
            set => SetValue(InColorProperty, value);
        }

        public enum LayoutEnum {None, TextOnly, Horizontal, Vertical, ImageOnly };
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
            "Layout", typeof(LayoutEnum), typeof(Button), new UIPropertyMetadata(LayoutEnum.TextOnly));
        public LayoutEnum Layout 
        {
            get => (LayoutEnum) GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

    }
}
