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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WindowManager.UserControls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WindowManagerPage : Page
    {
        private string SwapUri;
        private SettingsData settings;

        public List<int[]> intermediateRectangles;
        public List<List<int[]>> optimalFrames;

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

            //Read settings from Json into class variable - this must come after the above code to correct current directory
            settings = SettingsManager.DeserialiseSettingsJSON();

            // Initialise main window
            this.InitializeComponent();

            // Adjust layout
            //AdjustGridSize(settings);

            DisplayPanelsFromJSON(settings);

            // This needs to come before adding TV element or must include type check
            WireEventHandlers();

            InitialiseTv(settings);
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

        private void DisplayPanelsFromJSON(SettingsData settings)
        {
            WebPanel[] PanelsArray = { Panel1, Panel2, Panel3, Panel4, Panel5, Panel6, Panel7, Panel8, Panel9 };

            // Set the visibility of all panels to collapsed
            foreach (WebPanel webPanel in PanelsArray)
            {
                webPanel.Visibility = Visibility.Collapsed;
            }

            // Set Uri and make visible any panels that are included in the JSON
            foreach (Panel panelData in settings.Panels.GetPanelsArray())
            {
                if (panelData != null)
                {
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

        private void WebPanel_Close(object sender, RoutedEventArgs e)
        {
            WebPanel webPanel = sender as WebPanel;

            // uri to be added or removed
            Uri deltaUri = new Uri (webPanel.Source);
            bool isAdd = false;
            int screenPanel = 6;
            // Calculate these?
            int[] ColumnWidths = { 425, 425, 425 };
            int[] RowHeights = { 250, 250, 250 };

            // call during calibration and assign to global variables
            // rectangles ordered by area - is "intermediates" interchangeable with "rectangles"?
            intermediateRectangles = PanelAlgorithms.IntermediateRectangles(screenPanel, ColumnWidths, RowHeights);
            optimalFrames = PanelAlgorithms.OptimalFrames(intermediateRectangles);

            // 1. Prioritise URIs
            // 2. Identify layout
            Panel[] panelArray = settings.Panels.GetPanelsArray();
            List<Uri> UriListByPriority = PanelAlgorithms.UriPriority(deltaUri, intermediateRectangles, panelArray, isAdd);
            dynamic packedFrames = PanelAlgorithms.PackedFrames(UriListByPriority, optimalFrames);

            // PackedFrames is a dict where keys are strings of panel names e.g. "Panel1"
            // The value corresponding to that key is another dict where the keys are "uri", "ColumnSpan", and "RowSpan"
            Dictionary<string, Dictionary<string, object>>.KeyCollection PanelNames = packedFrames.Keys;

            foreach (var PanelNameString in PanelNames)
            {
                Uri uri = packedFrames[PanelNameString]["uri"];
                int ColumnSpan = packedFrames[PanelNameString]["ColumnSpan"];
                int RowSpan = packedFrames[PanelNameString]["RowSpan"];

                settings.Panels.SetPanelDataByName(PanelNameString, uri, ColumnSpan, RowSpan);

            }

            // 3. Write to JSON - function will only take SettingsData object
            SettingsManager.SerialiseSettingsJSON(settings);

            // write to json
            settings.Panels.ClosePanelByName(webPanel.Name);
            SettingsManager.SerialiseSettingsJSON(settings);

            webPanel.Visibility = Visibility.Collapsed;

            DisplayPanelsFromJSON(settings);
        }

        public void Add_WebPanel(object sender, Uri deltaUri)
        {
            // uri to be added or removed
            //Uri deltaUri = new Uri("https://www.microsoft.com");
            bool isAdd = true;
            int screenPanel = settings.Tv.PanelNum;
            // Calculate these?
            int[] ColumnWidths = { 425, 425, 425 };
            int[] RowHeights = { 250, 250, 250 };

            // call during calibration and assign to global variables
            // rectangles ordered by area - is "intermediates" interchangeable with "rectangles"?
            intermediateRectangles = PanelAlgorithms.IntermediateRectangles(screenPanel, ColumnWidths, RowHeights);
            optimalFrames = PanelAlgorithms.OptimalFrames(intermediateRectangles);

            // 1. Prioritise URIs
            // 2. Identify layout
            Panel[] panelArray = settings.Panels.GetPanelsArray();
            List<Uri> UriListByPriority = PanelAlgorithms.UriPriority(deltaUri, intermediateRectangles, panelArray, isAdd);
            dynamic packedFrames = PanelAlgorithms.PackedFrames(UriListByPriority, optimalFrames);

            // PackedFrames is a dict where keys are strings of panel names e.g. "Panel1"
            // The value corresponding to that key is another dict where the keys are "uri", "ColumnSpan", and "RowSpan"
            Dictionary<string, Dictionary<string, object>>.KeyCollection PanelNames = packedFrames.Keys;

            foreach(var PanelNameString in PanelNames)
            {
                Uri uri = packedFrames[PanelNameString]["uri"];
                int ColumnSpan = packedFrames[PanelNameString]["ColumnSpan"];
                int RowSpan = packedFrames[PanelNameString]["RowSpan"];

                settings.Panels.SetPanelDataByName(PanelNameString, uri, ColumnSpan, RowSpan);

            }

            // 3. Write to JSON - function will only take SettingsData object
            SettingsManager.SerialiseSettingsJSON(settings);

            // 4. Update panels from JSON
            DisplayPanelsFromJSON(settings);
        }


    }
}
