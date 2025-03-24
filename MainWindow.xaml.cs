
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LFFSSK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.ResizeMode = System.Windows.ResizeMode.NoResize;

            MaximizeToSecondaryMonitor();
            this.Topmost = GeneralVar.IsTopMost;
            this.Cursor = GeneralVar.IsHideCursor ? System.Windows.Input.Cursors.None : System.Windows.Input.Cursors.Arrow;
            //this.Width = GeneralVar.MainWidth;
            //this.Height = GeneralVar.MainHeight;

            if (GeneralVar.IsScreenMaximized)
            {

                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
                this.WindowState = System.Windows.WindowState.Normal;

            if(GeneralVar.Display_WindowStyle)
            {
                this.WindowStyle = System.Windows.WindowStyle.None;
            }
            else
            {
                this.WindowStyle = System.Windows.WindowStyle.ThreeDBorderWindow;
            }
        }

        public void MaximizeToSecondaryMonitor()
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => s.DeviceName == @"\\.\DISPLAY2").FirstOrDefault();

            if (secondaryScreen != null)
            {
                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;

                if (this.IsLoaded)
                {
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = System.Windows.WindowStyle.None;
                }
            }
        }

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(txtBarcode);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtBarcode);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GeneralVar.MainWindowVM.EasyMaintenanceCommand.Execute(null);
        }

        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
