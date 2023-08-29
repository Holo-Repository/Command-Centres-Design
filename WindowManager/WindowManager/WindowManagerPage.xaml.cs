using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WindowManager.UserControls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing.PrintTicket;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{

    public static class MinimumDimensions
    {
        public static double MinimumPanelHeight { get; private set; }
        public static double MinimumPanelWidth { get; private set; }

        public static void Initialize(double height, double width)
        {
            MinimumPanelHeight = height * 0.125; // 10% of screenheight
            MinimumPanelWidth = width * 0.125; // 10% of screenwidth
        }
    }

    public class OptimalFrameMembers
    {
        public static List<int[]> intermediateRectangles;
        public static List<List<int[]>> optimalFrames;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WindowManagerPage : Page
    {
        private string SwapUri;

        //globals to be set on start-up/calibration
        public int screenPanel;
        // Calculate these!
        public double[] ColumnWidths;
        public double[] RowHeights;

        // A dictionary to map panel number : row number
        private Dictionary<int, int> RowMappings = new Dictionary<int, int>()
        {
            { 1, 0 }, { 2, 0 }, { 3, 0 },
            { 4, 1 }, { 5, 1 }, { 6, 1 },
            { 7, 2 }, { 8, 2 }, { 9, 2 }
        };
        // A dictionary to map panel number : column number
        private Dictionary<int, int> ColumnMappings = new Dictionary<int, int>()
        {
            { 1, 0 }, { 2, 1 }, { 3, 2 },
            { 4, 0 }, { 5, 1 }, { 6, 2 },
            { 7, 0 }, { 8, 1 }, { 9, 2 }
        };

        public WindowManagerPage()
        {
            // Directory.GetCurrentDirectory was returning service directory of system32 so using this workaround instead
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectName = "WindowManager\\";
            int startOfProj = baseDir.LastIndexOf(projectName);

            string currentDir = baseDir.Substring(0, (startOfProj + projectName.Length));
            Directory.SetCurrentDirectory(currentDir);

            //Minimum panel dimensions - scaled to window size - see definition above
            MinimumDimensions.Initialize(MainWindow.settings.WindowDimensions.Height, MainWindow.settings.WindowDimensions.Width);

            if (screenPanel != MainWindow.settings.Tv.PanelNum)
            {
                screenPanel = MainWindow.settings.Tv.PanelNum;
                ColumnWidths = MainWindow.settings.Grid.ColumnWidths;
                RowHeights = MainWindow.settings.Grid.RowHeights;

                //initialise intermediate rectantles and optimal frames
                OptimalFrameMembers.intermediateRectangles = PanelAlgorithms.IntermediateRectangles(screenPanel, ColumnWidths, RowHeights, MinimumDimensions.MinimumPanelHeight, MinimumDimensions.MinimumPanelWidth);
                OptimalFrameMembers.optimalFrames = PanelAlgorithms.OptimalFrames(OptimalFrameMembers.intermediateRectangles);
            }


            // Initialise main window
            this.InitializeComponent();

            // overwrite settings object in case new file has been uploaded


            // Adjust layout
            AdjustGridSize(MainWindow.settings);

            DisplayPanelsFromJSON(MainWindow.settings);

            // This needs to come before adding TV element or must include type check
            WireEventHandlers();

            // If tv panel changes during calibration, will need to swap panel 
            InitialiseTv(MainWindow.settings);
        }

        private void AdjustGridSize(SettingsData settings)
        {
            double[] RowHeights = settings.Grid.RowHeights;
            double[] ColumnWidths = settings.Grid.ColumnWidths;

            RowDefinitionCollection rowDefinitions = PanelGrid.RowDefinitions;
            ColumnDefinitionCollection columnDefinitions = PanelGrid.ColumnDefinitions;

            for (int i = 0; i < rowDefinitions.Count; i++)
            {
                rowDefinitions[i].Height = new GridLength(RowHeights[i]);
            }

            for (int i = 0; i < columnDefinitions.Count; i++)
            {
                columnDefinitions[i].Width = new GridLength(ColumnWidths[i]);
            }

        }

        public void DisplayPanelsFromJSON(SettingsData settings)
        {
            WebPanel[] PanelsArray = { Panel1, Panel2, Panel3, Panel4, Panel5, Panel6, Panel7, Panel8, Panel9 };

            // Set the visibility of all panels to collapsed
            foreach (WebPanel webPanel in PanelsArray)
            {
                webPanel.Visibility = Visibility.Collapsed;
            }

            Panel[] PanelsFromJSON = settings.Panels.GetPanelsArray();

            // Set Uri and make visible any panels that are included in the JSON
            for (int i = 0; i < 9; i++)
            {
                if (PanelsFromJSON[i] != null)
                {
                    Panel panelData = PanelsFromJSON[i];

                    int index = panelData.PanelNum - 1;
                    WebPanel panel = PanelsArray[index];

                    panel.Visibility = Visibility.Visible;
                    panel.SetUri(new Uri(panelData.Uri));

                    Microsoft.UI.Xaml.Controls.Grid.SetRowSpan(panel, panelData.RowSpan);
                    Microsoft.UI.Xaml.Controls.Grid.SetColumnSpan(panel, panelData.ColumnSpan);

                }
            }

        }

        private void InitialiseTv(SettingsData settings)
        {
            // Create component and link PanelGrid as parent
            TvPanel tvPanel = new TvPanel();
            PanelGrid.Children.Add(tvPanel);

            int PanelNum = settings.Tv.PanelNum;

            int RowNumber = RowMappings[PanelNum];
            int ColumnNumber = ColumnMappings[PanelNum];

            Microsoft.UI.Xaml.Controls.Grid.SetRow(tvPanel, RowNumber);
            Microsoft.UI.Xaml.Controls.Grid.SetColumn(tvPanel, ColumnNumber);

        }

        public void WireEventHandlers()
        {
            UIElementCollection panels = PanelGrid.Children;
            foreach (WebPanel panel in panels)
            {
                panel.Frame_DragStarting += new TypedEventHandler<UIElement, DragStartingEventArgs>(WebPanel_DragStarting);
                panel.Frame_DragOver += new TypedEventHandler<object, DragEventArgs>(WebPanel_DragOver);
                panel.Frame_Drop += new TypedEventHandler<object, DragEventArgs>(WebPanel_Drop);
                panel.Frame_DropCompleted += new TypedEventHandler<UIElement, DropCompletedEventArgs>(WebPanel_DropCompleted);
                panel.Frame_PointerEntered += new TypedEventHandler<object, PointerRoutedEventArgs>(WebPanel_PointerEntered);
                panel.Frame_PointerExited += new TypedEventHandler<object, PointerRoutedEventArgs>(WebPanel_PointerExited);
                panel.AppBarButton_Click += new TypedEventHandler<object, RoutedEventArgs>(WebPanel_Close);
            }

            MainMenuBar.Add_Window += new TypedEventHandler<object, Uri>(Add_WebPanel);
            MainMenuBar.Toggle_Border_Visibility += new TypedEventHandler<object, bool>(Toggle_BorderVisibility);
        }

        // event handlers
        private void ReloadPanels(object sender, RoutedEventArgs e)
        {
            AdjustGridSize(MainWindow.settings);

            DisplayPanelsFromJSON(MainWindow.settings);
        }
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
            WebPanel webPanel = sender as WebPanel;
            webPanel.ChangeCommandBarVisibility("visible");

        }

        private void WebPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            WebPanel webPanel = sender as WebPanel;
            webPanel.ChangeCommandBarVisibility("collapsed");
        }

        private void WebPanel_Close(object sender, RoutedEventArgs e)
        {
            WebPanel webPanel = sender as WebPanel;

            // uri to be removed
            Uri deltaUri = new Uri (webPanel.Source);
            bool isAdd = false;                                                                                             

            Panel[] panelArray = MainWindow.settings.Panels.GetPanelsArray();

            // 1. Prioritise URIs
            List<Uri> UriListByPriority = PanelAlgorithms.UriPriority(deltaUri,OptimalFrameMembers.intermediateRectangles, panelArray, isAdd);

            if (UriListByPriority.Count == 0 )
            {
                MainWindow.settings.Panels.CloseAllPanels();
                DisplayPanelsFromJSON(MainWindow.settings);
                return;
            }

            // 2. Identify layout
            dynamic packedFrames = PanelAlgorithms.PackedFrames(UriListByPriority, OptimalFrameMembers.optimalFrames);

            // PackedFrames is a dict where keys are strings of panel names e.g. "Panel1"
            // The value corresponding to that key is another dict where the keys are "uri", "ColumnSpan", and "RowSpan"
            Dictionary<string, Dictionary<string, object>>.KeyCollection PanelNames = packedFrames.Keys;

            //kill all panels - make way for new
            MainWindow.settings.Panels.CloseAllPanels();

            foreach (var PanelNameString in PanelNames)
            {
                Uri uri = packedFrames[PanelNameString]["uri"];
                int ColumnSpan = packedFrames[PanelNameString]["ColumnSpan"];
                int RowSpan = packedFrames[PanelNameString]["RowSpan"];

                MainWindow.settings.Panels.SetPanelDataByName(PanelNameString, uri, ColumnSpan, RowSpan);

            }

            // 3. Write to JSON - function will only take SettingsData object
            //SettingsManager.SerialiseSettingsJSON(MainWindow.settings);

            //4. Display panels from JSON
            DisplayPanelsFromJSON(MainWindow.settings);

            MainMenuBar.DecrementNumWindows();

        }

        public void Add_WebPanel(object sender, Uri deltaUri)
        {
            bool isAdd = true;

            Panel[] panelArray = MainWindow.settings.Panels.GetPanelsArray();
            Panel[] fullPanels = panelArray.Where(x => x != null).ToArray();
            if (fullPanels.Length == OptimalFrameMembers.optimalFrames.Count) return; //prevents user from adding more panels than layout supports

            // 1. Prioritise URIs
            List<Uri> UriListByPriority = PanelAlgorithms.UriPriority(deltaUri, OptimalFrameMembers.intermediateRectangles, panelArray, isAdd);

            // 2. Identify layout
            dynamic packedFrames = PanelAlgorithms.PackedFrames(UriListByPriority, OptimalFrameMembers.optimalFrames);
            // PackedFrames is a dict where keys are strings of panel names e.g. "Panel1"
            // The value corresponding to that key is another dict where the keys are "uri", "ColumnSpan", and "RowSpan"
            Dictionary<string, Dictionary<string, object>>.KeyCollection PanelNames = packedFrames.Keys;

            //kill all panels - make way for new
            MainWindow.settings.Panels.CloseAllPanels();

            foreach (var PanelNameString in PanelNames)
            {
                Uri uri = packedFrames[PanelNameString]["uri"];
                int ColumnSpan = packedFrames[PanelNameString]["ColumnSpan"];
                int RowSpan = packedFrames[PanelNameString]["RowSpan"];

                MainWindow.settings.Panels.SetPanelDataByName(PanelNameString, uri, ColumnSpan, RowSpan);

            }

            // 3. Write to JSON - function will only take SettingsData object
            SettingsManager.SerialiseSettingsJSON(MainWindow.settings);

            // 4. Update panels from JSON
            DisplayPanelsFromJSON(MainWindow.settings);

            MainMenuBar.IncrementNumWindows();

        }

        public void Toggle_BorderVisibility(object sender, bool state)
        {
            WebPanel[] PanelsArray = { Panel1, Panel2, Panel3, Panel4, Panel5, Panel6, Panel7, Panel8, Panel9 };

            //Brush updatedBorder;
            Thickness updatedBorder;

            if (state)
            {
                //updatedBorder = new SolidColorBrush(Microsoft.UI.Colors.Black);
                updatedBorder = new Thickness(0.5);

            } else
            {
                //updatedBorder = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                updatedBorder = new Thickness(0);
            
            }

            foreach (WebPanel panel in PanelsArray)
            {
                Frame frame = panel.Content as Frame;
                Microsoft.UI.Xaml.Controls.Grid grid = frame.Content as Microsoft.UI.Xaml.Controls.Grid;
                grid.BorderThickness = updatedBorder;

            }
        }
    }
}
