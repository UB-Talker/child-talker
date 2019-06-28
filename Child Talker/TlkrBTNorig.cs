using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Child_Talker.TalkerViews;

namespace Child_Talker
{
    public partial class TlkrBTNorig : Button
    {
        /*
         *  when this Button is selected the autoscan stops cycling until 'go back' is pressed
         *     true - pause autoscan
         *     false - autoscan continues as normal     (default)
         */
        public static readonly DependencyProperty pauseOnSelectProperty = DependencyProperty.Register(
            "PauseOnSelect",                        // Name of property on XAML
            typeof(bool),                           // type of property on XAML
            typeof(TlkrBTN),                        // who inherits this property
            new UIPropertyMetadata(null));

        // TODO make functional
        /// <summary>
        /// !!!!!!! NOT YET IMPLEMENTED !!!!!!!
        /// Tells Autoscan whether or not to scan this element 
        /// <para>    true - autoscan will ignore this item </para>
        /// <para>   false - autoscan continues as normal     (default) </para>
        /// </summary>
        public static readonly DependencyProperty DoNotScanProperty = DependencyProperty.Register(
            "DoNotScan",                             // Name of property on XAML
            typeof(bool),                           // type of property on XAML
            typeof(TlkrBTN),                        // who inherits this property
            new UIPropertyMetadata(null));

        public bool PauseOnSelect
        {
            get => (bool)GetValue(pauseOnSelectProperty); 
            set => SetValue(pauseOnSelectProperty, value); 
        } 
        
        public bool DoNotScan 
        {
            get => (bool)GetValue(DoNotScanProperty); 
            set => SetValue(DoNotScanProperty, value); 
        }


    }
    public partial class TlkrBTNorig : Button 
    {
        private Dictionary<DependencyObject, Brush> foregroundObjects = new Dictionary<DependencyObject, Brush>();


        public void ResetTkrForeground()
        {
            foreach (KeyValuePair<DependencyObject, Brush> foregroundObject in foregroundObjects)
            {
                switch (foregroundObject.Key)
                {
                    case TlkrBTN button:
                        button.Foreground = Brushes.Cyan; 
                        break;
                    case Border border:
                        border.Background = foregroundObject.Value;
                        break;
                    case TextBlock textBlock:
                        textBlock.Foreground = foregroundObject.Value;
                        break;
                    case Label label:
                        label.Foreground = foregroundObject.Value;
                        break;
                }
            }
        }
        
        public Brush TkrForeground
        {
            set
            {
                foreach (KeyValuePair<DependencyObject, Brush> foregroundObject in foregroundObjects)
                {
                    switch (foregroundObject.Key)
                    {
                        case TlkrBTN button:
                            button.Foreground = value;
                            break;
                        case Border border:
                            border.Background = value;
                            break;
                        case TextBlock textBlock:
                            textBlock.Foreground = value;
                            break;
                        case Label label:
                            label.Foreground = value;
                            break;
                    }
                }
            }
            get => foregroundObjects[this];
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool Selected = false;

        public TlkrBTNorig()
        { 
            Brush foreground = (App.Current.Resources["ColorScheme.Foreground.Color"] as Brush);
            foregroundObjects.Add(this, foreground);
            this.Foreground = foreground;
            this.Initialized += (sender, e) => ForegroundCollection(sender as TlkrBTN, foregroundObjects); //occurs once immediately after object has been fully initialized
            this.DataContextChanged += OnContentChanged;
        }

        public void OnContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            foregroundObjects = new Dictionary<DependencyObject, Brush>
            {
                { this, this.Foreground }
            };
            ForegroundCollection(this, foregroundObjects);

        }

        

        private static void ForegroundCollection(DependencyObject parent, Dictionary<DependencyObject, Brush> logicalCollection)
        {
            if (parent == null) { return; }
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            
            foreach (var child in children)
            {
                if (child == null) { break; }

                switch (child)
                {
                    case Border border:
                        logicalCollection.Add(border, border.Background);
                        break;
                    case TextBlock textBlock:
                        logicalCollection.Add(textBlock, textBlock.Foreground);
                        break;
                    case Label label:
                        logicalCollection.Add(label, label.Foreground);
                        break;
                    default:
                        var depChild = child as DependencyObject;
                        ForegroundCollection(depChild, logicalCollection); //If still in dependencyobject, go into depChild's children
                        break;
                }
            }
        }



    }

}
