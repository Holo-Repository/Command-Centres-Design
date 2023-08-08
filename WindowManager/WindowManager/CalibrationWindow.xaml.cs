using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using Microsoft.UI.Xaml.Shapes;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using WinRT.Interop;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalibrationWindow : Window
    {
        private Point startPoint;
        private Rectangle currentRectangle;
        //private bool fillingChildren = false;
        private Dictionary<string, Rect> rectanglesDictionary = new Dictionary<string, Rect>();

        public CalibrationWindow()
        {
            this.InitializeComponent();
            canvas.PointerPressed += Canvas_PointerPressed;
            canvas.PointerMoved += Canvas_PointerMoved;
            canvas.PointerReleased += Canvas_PointerReleased;
            //canvas.KeyDown += Canvas_KeyDown;
            //canvas.Focus(FocusState.Keyboard);
            //TextBlock headingTextBlock = new TextBlock
            //{
            // Text = "Draw a rectangle around the TV in the center of the screen",
            //FontSize = 25,
            //Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
            // TextWrapping = TextWrapping.Wrap,
            //TextAlignment = TextAlignment.Center
            //};
            // canvas.Children.Add(headingTextBlock);

            // Position the TextBlock at the center of the canvas
            //double centerX = canvas.ActualWidth / 2 - headingTextBlock.ActualWidth / 2;
            //double centerY = canvas.ActualHeight / 2 - headingTextBlock.ActualHeight / 2;
            // Canvas.SetLeft(headingTextBlock, centerX);
            // Canvas.SetTop(headingTextBlock, centerY);

            AppWindow m_appWindow = GetAppWindowForCurrentWindow();
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            return AppWindow.GetFromWindowId(myWndId);

        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            canvas.Children.Clear();
            startPoint = e.GetCurrentPoint(canvas).Position;
            currentRectangle = new Rectangle
            {
                Fill = new SolidColorBrush(Microsoft.UI.Colors.Black),
                //StrokeThickness = 5,
                Opacity = 1,
            };
            Canvas.SetLeft(currentRectangle, startPoint.X);
            Canvas.SetTop(currentRectangle, startPoint.Y);
            canvas.Children.Add(currentRectangle);
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (currentRectangle == null)
            {
                return;
            }

            Point endPoint = e.GetCurrentPoint(canvas).Position;

            double width = Math.Abs(endPoint.X - startPoint.X);
            double height = Math.Abs(endPoint.Y - startPoint.Y);

            Canvas.SetLeft(currentRectangle, Math.Min(startPoint.X, endPoint.X));
            Canvas.SetTop(currentRectangle, Math.Min(startPoint.Y, endPoint.Y));

            currentRectangle.Width = width;
            currentRectangle.Height = height;
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // currentRectangle = null;
            //System.Diagnostics.Debug.WriteLine("good");
            if (currentRectangle != null)
            {
                rectanglesDictionary.Clear();
                // Get the coordinates and size of the rectangle
                Point endPoint = e.GetCurrentPoint(canvas).Position;
                double width = Math.Abs(endPoint.X - startPoint.X);
                double height = Math.Abs(endPoint.Y - startPoint.Y);

                double left = Math.Min(startPoint.X, endPoint.X);
                double top = Math.Min(startPoint.Y, endPoint.Y);

                // Create a Rect instance to represent the rectangle
                Rect rectangleRect = new Rect(left, top, width, height);

                // Convert the startPoint (which is a Point) to a string to use as the key
                string key = $"rectangle";

                // Add the rectangle to the dictionary
                rectanglesDictionary[key] = rectangleRect;

                currentRectangle = null;
            }
        }

        private void Canvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                if (rectanglesDictionary.Count > 0)
                {
                    foreach (var kvp in rectanglesDictionary)
                    {
                        Rect rectangleRect = kvp.Value;

                        // Get width and height of the rectangle
                        double width = rectangleRect.Width;
                        double height = rectangleRect.Height;
                        if (width != 0 & height != 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Value: {kvp.Value}");

                            SettingsData settings = DeserialiseSettingsJSON();

                            double tv_width = kvp.Value.Width;
                            double tv_height = kvp.Value.Height;
                            double tv_x = kvp.Value.X;
                            double tv_y = kvp.Value.Y;

                            settings.Tv.Height = tv_height;
                            settings.Tv.Width = tv_width;
                            settings.Tv.X_Position = tv_x;
                            settings.Tv.Y_Position = tv_y;

                            SerialiseSettingsJSON(settings);
                          

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Not a valid rectangle");
                        }

                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("There is no rectangle");
                }

            }

            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                AppWindow m_appWindow = GetAppWindowForCurrentWindow();
                m_appWindow.Destroy();
            }
        }

        private SettingsData DeserialiseSettingsJSON()
        {
            string dir = Directory.GetCurrentDirectory();
            string filePath = dir + "\\settings.json";

            string jsonString = File.ReadAllText(filePath);
            SettingsData settings = JsonSerializer.Deserialize<SettingsData>(jsonString);

            return settings;
        }

        private void SerialiseSettingsJSON(SettingsData newSettings)
        {
            string fileName = "settings.json";

            string jsonString = JsonSerializer.Serialize<SettingsData>(newSettings);
            File.WriteAllText(fileName, jsonString);



        }

    }
}

