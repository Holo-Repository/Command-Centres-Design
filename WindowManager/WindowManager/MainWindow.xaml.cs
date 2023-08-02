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
using Microsoft.UI.Xaml.Media.Imaging;

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

            Frame frame = sender as Frame;

            if (frame.Content != null) 
            { 
                // set payload of DataPackage to WebLink
                WebView2 webView = frame.Content as WebView2;
                args.Data.SetWebLink(webView.Source);

            }
            else { System.Diagnostics.Debug.WriteLine("null"); }

        }

        private void Frame_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;

        }

        private async void Frame_Drop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Drop event handler triggered");

            Frame frame2 = sender as Frame;

            if (e.DataView.Contains(StandardDataFormats.WebLink))
            {
                var webLink = await e.DataView.GetWebLinkAsync();
                if (webLink != null)
                {

                    // swap content
                    frame2.Content = new WebView2 { Source = webLink };

                    // Set margin of webView (for now)
                    WebView2 webView = frame2.Content as WebView2;
                    webView.Margin = new Thickness(10);

                }
            }
        }

        private void Frame_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("DropCompleted event handler triggered");
            
            // remove frame1 content
            Frame originalFrame = sender as Frame;
            originalFrame.Content = null;

        }

        private void Frame2_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Frame2_DragLeave(object sender, DragEventArgs e)
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
