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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager.UserControls
{
    public sealed partial class WebViewGrid : UserControl
    {
        public event TypedEventHandler<object, PointerRoutedEventArgs> WebViewGrid_PointerEntered;
        public event TypedEventHandler<object, PointerRoutedEventArgs> WebViewGrid_PointerExited;

        public void WebViewGrid1_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.WebViewGrid_PointerEntered != null)
                this.WebViewGrid_PointerEntered(this, e);
        }

        public void WebViewGrid1_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.WebViewGrid_PointerExited != null)
                this.WebViewGrid_PointerExited(this, e);
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(WebViewGrid), new PropertyMetadata(string.Empty));

        public WebViewGrid()
        {
            this.InitializeComponent();
        }

    }
}
