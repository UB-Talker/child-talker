using System;
using System.Collections.Generic;
using System.Globalization;
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
        
        
        /// <summary>
        /// A method to obtain the size of the incorporated textbox called BtnText and the height and width of the text it contains
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns> Tuple, Item1=Size of Text, Item2= Size of Holding TextBox</returns>
        protected Size MeasureString(string candidate)
        {
            TextBlock BtnText = (TextBlock)this.Template.FindName("BtnText", this);
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(BtnText.FontFamily, BtnText.FontStyle, BtnText.FontWeight, BtnText.FontStretch),
                BtnText.FontSize,
                System.Windows.Media.Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public void ResizeToText()
        {
            Size textsize = MeasureString(this.Text);
            var fullSize = this.Width + this.Margin.Left + this.Margin.Right;
            var textWidth = textsize.Width;
            var txtblockwidth = fullSize;
            /*while (textWidth>= txtblockwidth-60)
            {
                this.Dispatcher.Invoke( () =>
                {
                    this.Width += fullSize;
                });
                txtblockwidth += fullSize;
            }
            */

            if (textWidth >= txtblockwidth - 60)
            {
                this.Dispatcher.Invoke( () =>
                {
                    this.Width = textWidth + 60;
                });

            }
        }

    }

}
