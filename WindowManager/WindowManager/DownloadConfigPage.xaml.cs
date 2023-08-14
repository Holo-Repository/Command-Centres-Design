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
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadConfigPage : Page
    {
        public DownloadConfigPage()
        {
            this.InitializeComponent();
        }


        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickAFileOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".json"); //filter for only json files

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                //PickAFileOutputTextBlock.Text = "Picked file: " + file.Name;
                
                string searchDirectory = Directory.GetCurrentDirectory();
                string settingsFilePath = FindSettingsFile(searchDirectory);
                string settingsFolder = Path.GetDirectoryName(settingsFilePath);
                try
                {
                    // Move the selected file to the app's installation folder
                   Windows.Storage.StorageFolder destinationFolder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(settingsFolder);
                    // Move the selected file to the destination folder
                   Windows.Storage.StorageFile settings = await Windows.Storage.StorageFile.GetFileFromPathAsync(settingsFilePath);

                    // reading json content from original settings and storing panel number
                    string jsonContent = await FileIO.ReadTextAsync(settings);
                    SettingsData original_settings = JsonSerializer.Deserialize<SettingsData>(jsonContent);

                    int tvPanelNumber = original_settings.Tv.PanelNum;

                    // reading json content from new settings 
                    string jsonContent_new = await FileIO.ReadTextAsync(file);
                    SettingsData new_settings = JsonSerializer.Deserialize<SettingsData>(jsonContent_new);

                    int new_panel_number = new_settings.Tv.PanelNum;

                    //checking if layouts are compatible

                    if (tvPanelNumber == new_panel_number)
                    {
                        original_settings.Panels = new_settings.Panels;

                        string updatedJsonContent = JsonSerializer.Serialize(original_settings, new JsonSerializerOptions { WriteIndented = true });

                        // Overwrite the old settings file with the updated content
                        await FileIO.WriteTextAsync(settings, updatedJsonContent);


                        //await file.CopyAndReplaceAsync(settings);

                        PickAFileOutputTextBlock.Text = "Setting updated successfully.";
                    }

                    else
                    {
                        PickAFileOutputTextBlock.Text = "Incompatible file format.";
                    }

                    
                }
                catch (Exception ex)
                {
                    PickAFileOutputTextBlock.Text = "Error moving file: " + ex.Message;
                }
            }
            else
            {
                PickAFileOutputTextBlock.Text = "Operation cancelled.";
            }
        }

        static string FindSettingsFile(string directory)
        {
            string settingsFileName = "settings.json";
            string[] matchingFiles = Directory.GetFiles(directory, settingsFileName, SearchOption.AllDirectories);

            return matchingFiles.FirstOrDefault(); // Returns the first matching file or null if not found
        }
    }
}
