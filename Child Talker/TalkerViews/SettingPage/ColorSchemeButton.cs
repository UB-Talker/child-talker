using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Child_Talker.TalkerButton;

namespace Child_Talker.TalkerViews.SettingPage
{
    public partial class ColorSchemeButton : Button
    {
        public static DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(
            "HighlightForeground", typeof(Brush), typeof(ColorSchemeButton), new PropertyMetadata(null));
        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        public static DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register(
            "HighlightBackground", typeof(Brush), typeof(ColorSchemeButton), new PropertyMetadata(null));
        public Brush HighlightBackground
        {
            get => (Brush)GetValue(HighlightBackgroundProperty);
            set => SetValue(HighlightBackgroundProperty, value);
        }

        public static DependencyProperty HighlightBorderProperty = DependencyProperty.Register(
            "HighlightBorder", typeof(Brush), typeof(ColorSchemeButton), new PropertyMetadata(null));
        public Brush HighlightBorder
        {
            get => (Brush)GetValue(HighlightBorderProperty);
            set => SetValue(HighlightBorderProperty, value);
        }

        public static DependencyProperty PageBackgroundProperty = DependencyProperty.Register(
            "PageBackground", typeof(Brush), typeof(ColorSchemeButton), new PropertyMetadata(null));
        public Brush PageBackground 
        {
            get => (Brush)GetValue(PageBackgroundProperty);
            set => SetValue(PageBackgroundProperty, value);
        }
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ColorSchemeButton : Button
    {
        public ColorSchemeButton()
        {
        }
    }
}