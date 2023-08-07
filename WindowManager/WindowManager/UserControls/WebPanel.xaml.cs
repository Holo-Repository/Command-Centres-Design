using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager.UserControls
{
    public sealed partial class WebPanel : UserControl
    {
        public event TypedEventHandler<UIElement, DragStartingEventArgs> Frame_DragStarting;
        public event TypedEventHandler<object, DragEventArgs> Frame_DragOver;
        public event TypedEventHandler<object, DragEventArgs> Frame_Drop;
        public event TypedEventHandler<UIElement, DropCompletedEventArgs> Frame_DropCompleted;
        public event TypedEventHandler<object, PointerRoutedEventArgs> Frame_PointerEntered;
        public event TypedEventHandler<object, PointerRoutedEventArgs> Frame_PointerExited;

        public void Frame1_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_PointerEntered != null)
                this.Frame_PointerEntered(this, e);
        }

        public void Frame1_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_PointerExited != null)
                this.Frame_PointerExited(this, e);
        }

        public void Frame1_DragStarting(UIElement sender, DragStartingEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_DragStarting != null)
                this.Frame_DragStarting(this, e);
        }

        public void Frame1_DragOver(object sender, DragEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_DragOver != null)
                this.Frame_DragOver(this, e);
        }

        public void Frame1_Drop(object sender, DragEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_Drop != null)
                this.Frame_Drop(this, e);
        }

        public void Frame1_DropCompleted(UIElement sender, DropCompletedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.Frame_DropCompleted != null)
                this.Frame_DropCompleted(this, e);
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebPanel), new PropertyMetadata("https://www.google.com"));

        public void ChangeCommandBarVisibility (string visibility)
        {
            if (visibility == "visible")
            {
                CloseCommandBar.Visibility = Visibility.Visible;
                MoveCommandBar.Visibility = Visibility.Visible;
            }
            else if (visibility == "collapsed")
            {
                CloseCommandBar.Visibility = Visibility.Collapsed;
                MoveCommandBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Invalid arguments for ChangeCommandBarVisibility");
            }
        }

        public void SetUri(Uri uri)
        {
            Source = uri.ToString();
            WebViewContent.Source = uri;
        }

        public WebPanel()
        {
            this.InitializeComponent();
        }
    }
}
