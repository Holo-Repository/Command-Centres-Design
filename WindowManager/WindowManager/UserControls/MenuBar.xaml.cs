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

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Enter URL:";
            dialog.PrimaryButtonText = "Go";
           
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "URL";
            dialog.Content = textBox;

            dialog.PrimaryButtonClick += GoButtonClick;

            var result = await dialog.ShowAsync();

        }

        private void GoButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args)
        {
            // currently url must be entered with the format http://www....
            TextBox textBox = dialog.Content as TextBox;
            string UriString = textBox.Text;
            Uri deltaUri = new Uri(UriString);

            //Uri deltaUri = new Uri("https://www.microsoft.com");
            bool isAdd = true;
            int screenPanel = 5;
            // Calculate these?
            int[] ColumnWidths = { 100, 100, 100 };
            int[] RowHeights = { 100, 100, 100 };

            // rectangles ordered by area - is "intermediates" interchangeable with "rectangles"?
            List<int[]> rectangles = PanelAlgorithms.IntermediateRectangles(screenPanel, ColumnWidths, RowHeights);
            List<List<int[]>> optimalFrames = PanelAlgorithms.OptimalFrames(rectangles);

            List<Uri> UriListByPriority = PanelAlgorithms.UriPriority(deltaUri, rectangles, optimalFrames, isAdd);
            dynamic packed = PanelAlgorithms.PackedFrames(UriListByPriority, optimalFrames);

        }
    }
}
