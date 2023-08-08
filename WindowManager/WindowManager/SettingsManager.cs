using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WindowManager
{
    internal static class SettingsManager
    {
        public static SettingsData DeserialiseSettingsJSON()
        {
            string dir = Directory.GetCurrentDirectory();
            string filePath = dir + "\\settings.json";

            string jsonString = File.ReadAllText(filePath);
            SettingsData settings = JsonSerializer.Deserialize<SettingsData>(jsonString);

            return settings;
        }

        public static void SerialiseSettingsJSON(SettingsData newSettings)
        {
            string fileName = "settings.json";

            string jsonString = JsonSerializer.Serialize<SettingsData>(newSettings);
            File.WriteAllText(fileName, jsonString);

        }
    }
}
