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
using System.Net;

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

        private Dictionary<string, Rect> rectanglesDictionary = new Dictionary<string, Rect>();
        public static event TypedEventHandler<object, RoutedEventArgs> Reload_Panels;

        private double menuBarWidth = MainWindow.settings.WindowDimensions.Width / MainWindow.settings.WindowDimensions.ScalingFactor;
        private double navBarHeight = MainWindow.settings.WindowDimensions.Height / MainWindow.settings.WindowDimensions.ScalingFactor;

        public CalibrationWindow()
        {
            this.InitializeComponent();
            menuBar.Width = menuBarWidth;
            navBar.Height = navBarHeight;

            Canvas.SetZIndex(infoBar, 1); // On top
            Canvas.SetZIndex(menuBar, 0); // Below rectangle1

            infoBar.Width = menuBarWidth - 20;

            canvas.PointerPressed += Canvas_PointerPressed;
            canvas.PointerMoved += Canvas_PointerMoved;
            canvas.PointerReleased += Canvas_PointerReleased;

            AppWindow m_appWindow = GetAppWindowForCurrentWindow();
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

            Windows.Graphics.SizeInt32 Size = m_appWindow.Size;
            int WindowHeight = Size.Height;
            int WindowWidth = Size.Width;

            MainWindow.settings.WindowDimensions.Height = WindowHeight;
            MainWindow.settings.WindowDimensions.Width = WindowWidth;
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            return AppWindow.GetFromWindowId(myWndId);

        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //canvas.Children.Clear();

            // remove any existing retangle 
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                Rectangle child = canvas.Children[i] as Rectangle;
                if (child != null && child.Fill is SolidColorBrush solidColorBrush)
                {
                    if (solidColorBrush.Color == Colors.Black)
                    {
                        canvas.Children.RemoveAt(i);
                    }
                }
            }

            startPoint = e.GetCurrentPoint(canvas).Position;

            // if pointer is not in menubar
            if (startPoint.X > 50 && startPoint.Y > 75)
            {
                currentRectangle = new Rectangle
                {
                    Fill = new SolidColorBrush(Microsoft.UI.Colors.Black),
                    Opacity = 1,
                };
                Canvas.SetLeft(currentRectangle, startPoint.X);
                Canvas.SetTop(currentRectangle, startPoint.Y);
                canvas.Children.Add(currentRectangle);
            }
            
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

                            double tv_width = kvp.Value.Width;
                            double tv_height = kvp.Value.Height;
                            double tv_x = kvp.Value.X;
                            double tv_y = kvp.Value.Y;

                            MainWindow.settings.Tv.Height = tv_height;
                            MainWindow.settings.Tv.Width = tv_width;
                            MainWindow.settings.Tv.X_Position = tv_x;
                            MainWindow.settings.Tv.Y_Position = tv_y;

                            SettingsManager.SerialiseSettingsJSON(MainWindow.settings);

                            // Will need to reload main window at the end of this
                            CalculateGridDimensions();

                            infoBar.Message = "Calibration settings successfully saved. Press ESC to exit.";

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

        private void CalculateGridDimensions()
        {
            // create variables for readability
            Tv tv = MainWindow.settings.Tv;
            WindowDimensions windowDimensions = MainWindow.settings.WindowDimensions;

            // overwrite variables scaled to dpi
            double scaleFactor = windowDimensions.ScalingFactor;

            // UI dimensions in px (as set in XAML, so should be consistent across different window sizes)
            double MenuBarHeight = 35 + 40;
            double MenuBarWidth = 50;

            // scaled height and width of outer border of grid
            double GridHeight = (windowDimensions.Height / scaleFactor) - MenuBarHeight;
            double GridWidth = (windowDimensions.Width / scaleFactor) - MenuBarWidth;

            // write grid dimensions to settings to scale pages
            MainWindow.settings.Grid.Height = GridHeight;
            MainWindow.settings.Grid.Width = GridWidth;

            // tv coordinates from top left hand corner of grid (instead of window)
            tv.Y_Position = tv.Y_Position - MenuBarHeight;
            tv.X_Position = tv.X_Position - MenuBarWidth;

            // margins from outer border of grid to tv
            double topMargin = tv.Y_Position;
            double leftMargin = tv.X_Position;
            double bottomMargin = GridHeight - tv.Height - tv.Y_Position;
            double rightMargin = GridWidth - tv.Width - tv.X_Position;

            // set constraints
            double marginPercentage = 0.10;
            double minYMargin = GridHeight * marginPercentage;
            double minXMargin = GridWidth * marginPercentage;

            // calculate tv panel number - if tv position violates margin, assign as side or corner panel filling margin
            if (topMargin < minYMargin)
            {
                // panel is 1, 2, or 3
                if (leftMargin < minXMargin)
                {
                    // panel 1
                    tv.PanelNum = 1; // this is passed by ref so changes the settings object too
                    tv.Height = tv.Height + topMargin;
                    tv.Width = tv.Width + leftMargin;
                    tv.X_Position = 0;
                    tv.Y_Position = 0;
                }
                else if (rightMargin < minXMargin)
                {
                    // panel 3
                    tv.PanelNum = 3;
                    tv.Height = tv.Height + topMargin;
                    tv.Width = tv.Width + rightMargin;
                    tv.Y_Position = 0;

                }
                else
                {
                    // panel 2
                    tv.PanelNum = 2;
                    tv.Height = tv.Height + topMargin;
                    tv.Y_Position = 0;

                }

            }
            else if (leftMargin < minXMargin)
            {
                // panel 4 or 7

                if (bottomMargin < minYMargin)
                {
                    // panel 7
                    tv.PanelNum = 7;
                    tv.Height = tv.Height + bottomMargin;
                    tv.Width = tv.Width + leftMargin;
                    tv.X_Position = 0;

                }
                else
                {
                    // panel 4
                    tv.PanelNum = 4;
                    tv.Width = tv.Width + leftMargin;
                    tv.X_Position = 0;
                }

            }
            else if (rightMargin < minXMargin)
            {
                // panel 6 or 9
                if (bottomMargin < minYMargin)
                {
                    // panel 9
                    tv.PanelNum = 9;
                    tv.Height = tv.Height + bottomMargin;
                    tv.Width = tv.Width + rightMargin;

                }
                else
                {
                    // panel 6
                    tv.PanelNum = 6;
                    tv.Width = tv.Width + rightMargin;
                }
            }
            else if (bottomMargin < minYMargin)
            {
                // panel 8
                tv.PanelNum = 8;
                tv.Height = tv.Height + bottomMargin;

            }
            else
            {
                // panel 5
                tv.PanelNum = 5;
            }

            // the height and widths of the remaining columns after space allocated for tv
            double nonTvRowHeight = (GridHeight - tv.Height) / 2;
            double nonTvColumnWidth = (GridWidth - tv.Width) / 2;

            double[] rowHeights = new double[3];
            double[] columnWidths = new double[3];

            // set row heights
            if (tv.PanelNum == 1 || tv.PanelNum == 2 || tv.PanelNum == 3)
            {
                rowHeights[0] = tv.Height;
                rowHeights[1] = nonTvRowHeight;
                rowHeights[2] = nonTvRowHeight;

            }
            else if (tv.PanelNum == 4 || tv.PanelNum == 5 || tv.PanelNum == 6)
            {
                rowHeights[0] = tv.Y_Position;
                rowHeights[1] = tv.Height;
                rowHeights[2] = GridHeight - tv.Y_Position - tv.Height;

            }
            else if (tv.PanelNum == 7 || tv.PanelNum == 8 || tv.PanelNum == 9)
            {
                rowHeights[0] = nonTvRowHeight;
                rowHeights[1] = nonTvRowHeight;
                rowHeights[2] = tv.Height;

            }
            else
            {
                throw new ArgumentException("Invalid panel number", tv.PanelNum.ToString());
            }

            // set column widths
            if (tv.PanelNum == 1 || tv.PanelNum == 4 || tv.PanelNum == 7)
            {
                columnWidths[0] = tv.Width;
                columnWidths[1] = nonTvColumnWidth;
                columnWidths[2] = nonTvColumnWidth;

            }
            else if (tv.PanelNum == 2 || tv.PanelNum == 5 || tv.PanelNum == 8)
            {
                columnWidths[0] = tv.X_Position;
                columnWidths[1] = tv.Width;
                columnWidths[2] = GridWidth - tv.X_Position - tv.Width;

            }
            else if (tv.PanelNum == 3 || tv.PanelNum == 6 || tv.PanelNum == 9)
            {
                columnWidths[0] = nonTvColumnWidth;
                columnWidths[1] = nonTvColumnWidth;
                columnWidths[2] = tv.Width;

            }
            else
            {
                throw new ArgumentException("Invalid panel number", tv.PanelNum.ToString());
            }

            MainWindow.settings.Grid.RowHeights = rowHeights;
            MainWindow.settings.Grid.ColumnWidths = columnWidths;

            // write settings to json
            SettingsManager.SerialiseSettingsJSON(MainWindow.settings);

        }


    }
}

