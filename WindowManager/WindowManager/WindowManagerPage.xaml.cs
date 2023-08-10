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
using WindowManager.UserControls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{

    public List<int[]> intermediateRectangles;
    public List<List<int[]>> optimalRectangles;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WindowManagerPage : Page
    {
        private string SwapUri;
        private SettingsData settings;

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
            AdjustGridSize(settings);

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

            MainMenuBar.GoButton_Click += new TypedEventHandler<object, RoutedEventArgs>(Add_WebPanel);
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
            webPanel.Visibility = Visibility.Collapsed;

            // write to json
            settings.Panels.ClosePanelByName(webPanel.Name);
            SettingsManager.SerialiseSettingsJSON(settings);
        }

        private void Add_WebPanel(object sender, RoutedEventArgs e)
        {
            // 1. Prioritise URIs
            // 2. Identify layout
            // 3. Write to JSON
            // 4. Update panels from JSON
            DisplayPanelsFromJSON(settings);
        }


    }
}
