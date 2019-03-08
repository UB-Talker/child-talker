﻿using System;
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
using System.Xml.Serialization;
using Child_Talker.TalkerViews;

namespace Child_Talker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Stack<TalkerView> previousViews;


        public MainWindow()
        {

            InitializeComponent();

            previousViews = new Stack<TalkerView>();

            TalkerView startScreen = new MainMenu();
            DataContext = startScreen;
            previousViews.Push(startScreen);
        }


        /*
         * Change the view to the previous view. The views will be maintained in a stack of TalkerViews.
         */
        public void back()
        {
            DataContext = previousViews.Peek();
            previousViews.Pop();
        }


        /*
         * Sets the previous view to a reference of a TalkerView.
         * This view is pushed to the top of the previousViews Stack
         */
        public void setPreviousView(TalkerView view)
        {
            previousViews.Push(view);
        }


        /*
         * Resets the Stack of TalkerViews. Used when the user returns straight to the MainMenu
         */
        public void resetStack()
        {
            previousViews = new Stack<TalkerView>();
        }
    }
}
