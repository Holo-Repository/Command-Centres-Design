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
using Windows.UI.ApplicationSettings;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Devices.Enumeration;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // settings object is initialise in the main window - maybe this should change
        public static SettingsData settings;

        public MainWindow()
        {
            // Directory.GetCurrentDirectory was returning service directory of system32 so using this workaround instead
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectName = "WindowManager\\";
            int startOfProj = baseDir.LastIndexOf(projectName);

            string currentDir = baseDir.Substring(0, (startOfProj + projectName.Length));
            Directory.SetCurrentDirectory(currentDir);

            //Read settings from Json into class variable - this must come after the above code to correct current directory
            settings = SettingsManager.DeserialiseSettingsJSON();

            SettingsData test = new SettingsData();
            // Initialise main window
            this.InitializeComponent();

            //// C# code to set AppTitleBar uielement as titlebar
            Window window = this;
            window.ExtendsContentIntoTitleBar = true;  // enable custom titlebar
            window.SetTitleBar(AppTitleBar);      // set user ui element as titlebar

            double AppBarWidth = AppTitleBar.Width;

            // Set fullscreen, get size of window and set scale factor
            AppWindow m_appWindow = GetAppWindowForCurrentWindow();
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

            Windows.Graphics.SizeInt32 Size = m_appWindow.Size;
            int WindowHeight = Size.Height;
            int WindowWidth = Size.Width;

            settings.WindowDimensions.Height = WindowHeight;
            settings.WindowDimensions.Width = WindowWidth;


        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            [DllImport("user32.dll")]
            static extern int GetDpiForWindow(IntPtr hwnd);

            // dots per inch
            int dpi = GetDpiForWindow(hWnd);

            // Calculate the scaling factor
            double scalingFactor = dpi / 96.0f;

            settings.WindowDimensions.ScalingFactor = scalingFactor;

            return AppWindow.GetFromWindowId(myWndId);

        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(typeof(WindowManagerPage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate(typeof(WindowManagerPage), args.RecommendedNavigationTransitionInfo);
                ScrollViewer scroll = sender.Content as ScrollViewer;
            }
            else if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
                args.InvokedItemContainer.UpdateLayout();
            }
        }

        private void NavView_Navigate(Type navPageType, NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                ContentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void CalibrationComplete()
        {

        }

        private void NavigationViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Window Manager Page reloaded");
        }
    }
}
