using System;
using System.Configuration;

namespace REVIT_GW_WPFAddin
{
    /// <summary>
    /// Helper class for sharing data across classes
    /// </summary>
    public static class Globals
    {
        public static string RevitVersion;
        public static string ToolVersion => GetAppSettings("toolversion");
        public static string KnowHelpLink => GetAppSettings("knowHelpLink");

        /// <summary>
        /// Gets the application settings from the dll config file.
        /// </summary>
        private static string GetAppSettings(string key)
        {
            Configuration config = GetGlobalsConfiguration();
            var element = config.AppSettings.Settings[key];
            var value = element?.Value;
            return !string.IsNullOrEmpty(value) ? string.Format(value, RevitVersion) : string.Empty;
        }

        private static Configuration GetGlobalsConfiguration()
        {
            string dllPath = typeof(Globals).Assembly.Location;
            try
            {
                return ConfigurationManager.OpenExeConfiguration(dllPath);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}