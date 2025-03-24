﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XFUtility.Keyboard.Controls;

namespace LFFSSK.View
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : UserControl
    {
        public AuthenticationView()
        {
            OnScreenKey.keyTextColor = (Brush)new BrushConverter().ConvertFromString("#FF000000");
            OnScreenKey.keyDownTextColor = (Brush)new BrushConverter().ConvertFromString("#FF000000");
            OnScreenKey.keyOusideBorder = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFD3D3D3");
            OnScreenKey.keyDownAnimationSurfaceColor = (Brush)new BrushConverter().ConvertFromString("White");
            OnScreenKey.keyTextHorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            OnScreenKey.keyTextVerticalAlignment = System.Windows.VerticalAlignment.Center;
            OnScreenKey.keyTextColor = (Brush)new BrushConverter().ConvertFromString("White");

            InitializeComponent();
            
        }


    }
}
