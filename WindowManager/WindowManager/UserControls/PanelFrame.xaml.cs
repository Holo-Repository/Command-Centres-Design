using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics.Tracing;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager.UserControls
{
    public sealed partial class PanelFrame : UserControl
    {
        public event TypedEventHandler<UIElement, DragStartingEventArgs> Frame_DragStarting;

        public void Frame1_DragStarting(UIElement sender, DragStartingEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_DragStarting != null)
                this.Frame_DragStarting(this, e);
        }
        public PanelFrame()
        {
            this.InitializeComponent();
        }
        //private void Frame_DragStarting(UIElement sender, DragStartingEventArgs args)
        //{

        //    Frame frame = sender as Frame;

        //    // if sender frame contains grid
        //    if (frame.Content != null & frame.Content.GetType() == typeof(Grid))
        //    {
        //        Grid grid = frame.Content as Grid;

        //        // if grid contains webView2 child
        //        if (grid.Children.Count > 0 & grid.Children[1].GetType() == typeof(WebView2))
        //        {
        //            // set payload of DataPackage to WebLink
        //            WebView2 webView = grid.Children[1] as WebView2;
        //            args.Data.SetWebLink(webView.Source);

        //        }

        //    }
        //    else { System.Diagnostics.Debug.WriteLine("null"); }

        //}

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

        private void Frame_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Pointer entered");

            //Frame frame = sender as Frame;
            //Grid grid = frame.Content as Grid;

            //if (grid.Children.Count > 0 & grid.Children[0].GetType() == typeof(RelativePanel))
            //{
            //    RelativePanel relativePanel = grid.Children[0] as RelativePanel;

            //    if (relativePanel.Children.Count > 0)
            //    {
            //        foreach (var child in relativePanel.Children)
            //        {
            //            if (child.GetType() == typeof(CommandBar))
            //            {
            //                child.Visibility = Visibility.Visible;
            //            }
            //        }
            //    }
            //}

        }

        private void Frame_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Pointer exited");

            //Frame frame = sender as Frame;
            //Grid grid = frame.Content as Grid;

            //if (grid.Children.Count > 0 & grid.Children[0].GetType() == typeof(RelativePanel))
            //{
            //    RelativePanel relativePanel = grid.Children[0] as RelativePanel;

            //    if (relativePanel.Children.Count > 0)
            //    {
            //        foreach (var child in relativePanel.Children)
            //        {
            //            if (child.GetType() == typeof(CommandBar))
            //            {
            //                child.Visibility = Visibility.Collapsed;
            //            }
            //        }
            //    }
            //}
        }
    }
}
