using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Initialising");

        }

        private void Frame_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Drag event started");
            System.Diagnostics.Debug.WriteLine(sender);
            System.Diagnostics.Debug.WriteLine(args);

            Frame frame = sender as Frame;
            if (frame.Content != null) { System.Diagnostics.Debug.WriteLine(frame.Content.ToString()); }
            else { System.Diagnostics.Debug.WriteLine(null); }

            // Create a DataPackage
            DataPackage dataPackage = new DataPackage();

        }

        private void Frame_DragOver(object sender, DragEventArgs e)
        {

        }

        private void Frame_Drop(object sender, DragEventArgs e)
        {

        }

        private void Frame_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {

        }

        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Uri targetUri = new Uri(addressBar.Text);
        //        MyWebView.Source = targetUri;
        //    }
        //    catch (FormatException ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}


    }
}
