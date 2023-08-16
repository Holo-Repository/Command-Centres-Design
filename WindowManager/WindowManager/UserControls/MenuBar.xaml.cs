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
        //public event TypedEventHandler<object, RoutedEventArgs> MenuFlyoutItem_Click;
        public event TypedEventHandler<object, Uri> Add_Window;

        public MenuBar()
        {
            this.InitializeComponent();
        }

        // This prompts the dialog pop-up when you click on Add by URI
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

            //bubble the event up to the parent
            if (this.Add_Window != null)
                this.Add_Window(this, deltaUri);

        }

        // Send the correct uri according to content
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            Uri deltaUri;

            switch (menuFlyoutItem.Text)
            {
                case "Bing":
                    deltaUri = new Uri("https://www.bing.com");
                    break;
                case "Youtube":
                    deltaUri = new Uri("https://www.youtube.com");
                    break;
                case "NHS England":
                    deltaUri = new Uri("https://www.england.nhs.uk/");
                    break;
                case "Create A Meeting":
                    deltaUri = new Uri("http://localhost:3000/createcall");
                    break;
                case "Join A Meeting":
                    deltaUri = new Uri("http://localhost:3000/");
                    break;
                default:
                    throw new ArgumentException("MenuFlyoutItem.Text matches no Uri", nameof(menuFlyoutItem.Text));

            }

            //bubble the event up to the parent
            if (this.Add_Window != null)
                this.Add_Window(this, deltaUri);
        }

    }
}
