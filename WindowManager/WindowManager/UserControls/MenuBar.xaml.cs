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
    public sealed partial class MenuBar : UserControl
    {
        public event TypedEventHandler<object, RoutedEventArgs> MenuFlyoutItem_Click;
        public event TypedEventHandler<object, RoutedEventArgs> GoButton_Click;

        public void MenuFlyoutItem1_Click(object sender, RoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.MenuFlyoutItem_Click != null)
                this.MenuFlyoutItem_Click(this, e);
        }

        public MenuBar()
        {
            this.InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //bubble the event up to the parent
            if (this.GoButton_Click != null)
                this.GoButton_Click(this, e);
        }
    }
}
