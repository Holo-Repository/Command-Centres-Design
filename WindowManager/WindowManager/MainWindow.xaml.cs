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
using WindowManager.UserControls;
using Microsoft.UI;
using WinRT.Interop;
using Windows.UI.WindowManagement;
using System.Reflection.Metadata;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using System.Text.Json;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private string SwapUri;
        public MainWindow()
        {
            this.InitializeComponent();
            
           // Directory.GetCurrentDirectory was returning service directory of system32 so using this workaround instead
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectName = "WindowManager\\";
            int startOfProj = baseDir.LastIndexOf(projectName);

            string currentDir = baseDir.Substring(0, (startOfProj + projectName.Length));
            Directory.SetCurrentDirectory(currentDir);

            // Wire event handlers (probably should put this in its own function)

            List<WebPanel> populateFrames = new List<WebPanel> { Panel1, Panel2, Panel3, Panel4, Panel5, Panel6 };
            foreach (WebPanel panel in populateFrames)
            {
                panel.Frame_DragStarting += new TypedEventHandler<UIElement, DragStartingEventArgs>(WebPanel_DragStarting);
                panel.Frame_DragOver += new TypedEventHandler<object, DragEventArgs>(WebPanel_DragOver);
                panel.Frame_Drop += new TypedEventHandler<object, DragEventArgs>(WebPanel_Drop);
                panel.Frame_DropCompleted += new TypedEventHandler<UIElement, DropCompletedEventArgs>(WebPanel_DropCompleted);
                panel.Frame_PointerEntered += new TypedEventHandler<object, PointerRoutedEventArgs>(WebPanel_PointerEntered);
                panel.Frame_PointerExited += new TypedEventHandler<object, PointerRoutedEventArgs>(WebPanel_PointerExited);
            }

            MainMenuBar.MenuFlyoutItem_Click += new TypedEventHandler<object, RoutedEventArgs>(Calibration_Click);

            System.Diagnostics.Debug.WriteLine("Initialising");

        }

        // event handlers
        private void WebPanel_DragStarting(UIElement sender, DragStartingEventArgs args)
        {

            WebPanel panel = sender as WebPanel;
            Uri panel_uri = new Uri(panel.Source);

            // set payload of DataPackage to WebLink
            args.Data.SetWebLink(panel_uri);

        }

        private void WebPanel_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;

        }

        private async void WebPanel_Drop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Drop event handler triggered");

            WebPanel panel = sender as WebPanel;

            if (e.DataView.Contains(StandardDataFormats.WebLink))
            {
                var webLink = await e.DataView.GetWebLinkAsync();
                if (webLink != null)
                {
                    // getter returns a string
                    SwapUri = panel.Source;
                    panel.SetUri(webLink);

                }
            }
        }

        private void WebPanel_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("DropCompleted event handler triggered");
            
            // remove frame1 content
            WebPanel panel = sender as WebPanel;
            panel.SetUri(new Uri(SwapUri));

        }

        private void WebPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pointer entered");

            WebPanel webPanel = sender as WebPanel;
            webPanel.ChangeCommandBarVisibility("visible");

        }

        private void WebPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pointer exited");

            WebPanel webPanel = sender as WebPanel;
            webPanel.ChangeCommandBarVisibility("collapsed");
        }

        private void Calibration_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow calibrationWindow = new CalibrationWindow();
            calibrationWindow.Activate();
        }

    }
}
