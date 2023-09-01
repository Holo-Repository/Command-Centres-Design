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
    public class NumWindowsData
    {
        public int _int { get; set; }
        public string _string { get; set; }

        public NumWindowsData()
        {
            _int = CountPanelsFromJson();

            if (_int == 8)
            {
                _string = "Max";
            }
            else
            {
                _string = _int.ToString();
            }

        }

        public int CountPanelsFromJson()
        {
            int NumPanels = 0;

            foreach (Panel panel in MainWindow.settings.Panels.GetPanelsArray())
            {
                if (panel != null)
                {
                    NumPanels++;
                }
            }

            return NumPanels;
        }
    }
    public sealed partial class MenuBar : UserControl
    {
        public event TypedEventHandler<object, Uri> Add_Window;
        public event TypedEventHandler<object, bool> Toggle_Border_Visibility;

        public NumWindowsData NumWindows { get; set; }

        public MenuBar()
        {
            this.InitializeComponent();
            this.NumWindows = new NumWindowsData();
            NumPanelsConditionalFormatting();
        }

        public void IncrementNumWindows()
        {
            this.NumWindows._int++;
            this.NumWindows._string = this.NumWindows._int.ToString();

            NumWindowsTextBlock.Text = this.NumWindows._string;

            NumPanelsConditionalFormatting();

        }

        public void DecrementNumWindows()
        {
            this.NumWindows._int--;
            this.NumWindows._string = this.NumWindows._int.ToString();

            NumWindowsTextBlock.Text = this.NumWindows._string;

            NumPanelsConditionalFormatting();
        }

        public void NumPanelsConditionalFormatting()
        {
            if (this.NumWindows._int == 8)
            {
                NumWindowsTextBlock.Text = "Max";
            }

            if (NumWindows._int < 8)
            {
                menuBar.IsEnabled = true;
            }
            else
            {
                menuBar.IsEnabled = false;
            }

        }

        private void GoButtonClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args)
        {
            TextBox textBox = dialog.Content as TextBox;
            string uriString = textBox.Text;

            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Valid URL entered
                if (this.Add_Window != null)
                    this.Add_Window(this, uriResult);
            }
            else
            {
                // Invalid URL entered
                InvalidWebPanelURL.IsOpen = true;
            }

        }

        // Send the correct uri according to content for website shortcuts
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
                    deltaUri = new Uri(MainWindow.settings.TeamsURIs.Create);
                    break;
                case "Join A Meeting":
                    deltaUri = new Uri(MainWindow.settings.TeamsURIs.Join);
                    break;
                default:
                    throw new ArgumentException("MenuFlyoutItem.Text matches no Uri", nameof(menuFlyoutItem.Text));

            }

            //bubble the event up to the parent
            if (this.Add_Window != null)
                this.Add_Window(this, deltaUri);
        }

        // add web panel by url, change create and join teams urls
        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            string dialogTitle;

            if (menuFlyoutItem.Text == "Add by URL")
            {
                dialogTitle = "Enter Web Panel URL:";
                dialog.PrimaryButtonClick += GoButtonClick;

            }
            else if (menuFlyoutItem.Text == "Create A Meeting URL")
            {
                dialogTitle = "Enter Create A Meeting Page URL:";
                dialog.PrimaryButtonClick += ChangeCreateURLClick;

            }
            else if (menuFlyoutItem.Text == "Join A Meeting URL")
            {
                dialogTitle = "Enter Join A Meeting URL: ";
                dialog.PrimaryButtonClick += ChangeJoinURLClick;

            }
            else
            {
                throw new ArgumentException("MenuFlyoutItem.Text invalid", nameof(menuFlyoutItem.Text));
            }

            dialog.Title = dialogTitle;
            dialog.PrimaryButtonText = "Go";

            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "https://";
            dialog.Content = textBox;

            var result = await dialog.ShowAsync();

        }

        private void ChangeJoinURLClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args)
        {
            TextBox textBox = dialog.Content as TextBox;
            string joinUriString = textBox.Text;

            if (Uri.TryCreate(joinUriString, UriKind.Absolute, out Uri uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Valid URL entered
                MainWindow.settings.TeamsURIs.Join = joinUriString;
                SettingsManager.SerialiseSettingsJSON(MainWindow.settings);
                // show success message
                ChangeJoin_Success.IsOpen = true;
            }
            else
            {
                // Invalid URL entered
                InvalidJoinURL.IsOpen = true;
            }
            
        }

        private void ChangeCreateURLClick(ContentDialog dialog, ContentDialogButtonClickEventArgs args)
        {
            TextBox textBox = dialog.Content as TextBox;
            string createUriString = textBox.Text;

            if (Uri.TryCreate(createUriString, UriKind.Absolute, out Uri uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                // Valid URL entered
                MainWindow.settings.TeamsURIs.Create = createUriString;
                SettingsManager.SerialiseSettingsJSON(MainWindow.settings);
                // show success message
                ChangeCreate_Success.IsOpen = true;
            }
            else
            {
                // Invalid URL entered
                InvalidCreateURL.IsOpen = true;
            }

        }

        // toggle borders on and off
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggle = sender as ToggleSwitch;
            bool state = toggle.IsOn;

            //bubble the event up to the parent
            if (this.Toggle_Border_Visibility != null)
                this.Toggle_Border_Visibility(this, state);
        }

        private async void InvalidWebPanelURL_TryAgain(TeachingTip sender, object args)
        {
            InvalidWebPanelURL.IsOpen = false;

            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Enter URL:";
            dialog.PrimaryButtonText = "Go";

            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "https://";
            dialog.Content = textBox;

            dialog.PrimaryButtonClick += GoButtonClick;

            var result = await dialog.ShowAsync();
        }

        private async void InvalidCreateURL_TryAgain(TeachingTip sender, object args)
        {
            InvalidCreateURL.IsOpen = false;

            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Enter Create A Meeting Page URL:";
            dialog.PrimaryButtonText = "Go";

            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "https://";
            dialog.Content = textBox;

            dialog.PrimaryButtonClick += ChangeCreateURLClick;

            var result = await dialog.ShowAsync();
        }

        private async void InvalidJoinURL_TryAgain(TeachingTip sender, object args)
        {
            InvalidJoinURL.IsOpen = false;

            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Enter Join A Meeting Page URL:";
            dialog.PrimaryButtonText = "Go";

            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "https://";
            dialog.Content = textBox;

            dialog.PrimaryButtonClick += ChangeJoinURLClick;

            var result = await dialog.ShowAsync();
        }
    }
}