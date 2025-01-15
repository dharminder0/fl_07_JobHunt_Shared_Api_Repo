
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using VendersCloud.Common.Configuration;

namespace VendersCloud.Common.Settings
{
    public static class GlobalSettings
    {
        private static NameValueCollection _appSettings = ConfigurationManager.AppSettings;
        public static string AllowedOrigins => GetString("AllowedOrigins");
        public static string DefaultConnectionName => GetString("DefaultConnectionName");
        public static string FilePath => GetPath("FilePath");
        public static string GetKeyValues(string key)
        {
            return !string.IsNullOrWhiteSpace(_appSettings[key]) ? _appSettings[key] : null;
        }

        public static string GetPath(string key)
        {
            return !string.IsNullOrWhiteSpace(_appSettings[key]) ? _appSettings[key] : null;
        }
        #region Utils
        private static string GetString(string key, string defaultVal = null)
        {
            // Fetch value from environment variables or _appSettings
            var data = Environment.GetEnvironmentVariable(key) ?? _appSettings[key] ?? defaultVal;

            if (data == null)
            {
                var filepath = GlobalSettings.FilePath;
                Console.WriteLine("Checking file at path: " + filepath);

                if (File.Exists(filepath))
                {
                    var json = File.ReadAllText(filepath);
                    var jsonObj = JObject.Parse(json);
                    var appSettings = jsonObj["AppSettings"];  // Ensure this matches the JSON structure

                    if (appSettings != null && appSettings[key] != null)
                    {
                        data = appSettings[key]?.ToString();
                    }
                    else
                    {
                        Console.WriteLine("Key not found in JSON: " + key);
                    }
                }
                else
                {
                    Console.WriteLine("File does not exist: " + filepath);
                }
            }

            return data;
        }



        private static int GetInt(string key, int defaultVal = 0)
        {
            var dataStr = Environment.GetEnvironmentVariable(key) ?? _appSettings[key];
            var data = dataStr != null ? Convert.ToInt32(dataStr) : defaultVal;
            if (dataStr == null)
            {
                var filepath = GlobalSettings.FilePath;
                if (File.Exists(filepath))
                {
                    var json = File.ReadAllText(filepath);
                    var jsonObj = JObject.Parse(json);
                    var appSettings = jsonObj["appSettings"];
                    if (appSettings != null && appSettings[key] != null)
                    {
                        data = Convert.ToInt32(appSettings[key]);
                    }
                    else
                    {
                        Console.WriteLine("Key not found: " + key);
                    }
                }
            }
            return data;
        }

        private static bool GetBool(string key, bool defaultVal = false)
        {
            var dataStr = Environment.GetEnvironmentVariable(key) ?? _appSettings[key];
            var data = dataStr != null ? Convert.ToBoolean(dataStr) : defaultVal;
            if (dataStr == null)
            {
                var filepath = GlobalSettings.FilePath;
                if (File.Exists(filepath))
                {
                    var json = File.ReadAllText(filepath);
                    var jsonObj = JObject.Parse(json);
                    var appSettings = jsonObj["appSettings"];
                    if (appSettings != null && appSettings[key] != null)
                    {
                        data = Convert.ToBoolean(appSettings[key]);
                    }
                    else
                    {
                        Console.WriteLine("Key not found: " + key);
                    }
                }
            }
            return data;
        }

        private static IEnumerable<string> GetListValues(string key, IEnumerable<string> defaultVal = null)
        {
            var dataStr = Environment.GetEnvironmentVariable(key) ?? _appSettings[key];
            var data = dataStr != null ? dataStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : defaultVal?.ToList();
            if (data == null)
            {
                var filepath = GlobalSettings.FilePath;
                if (File.Exists(filepath))
                {
                    var json = File.ReadAllText(filepath);
                    var jsonObj = JObject.Parse(json);
                    var appSettings = jsonObj["appSettings"];
                    if (appSettings != null && appSettings[key] != null)
                    {
                        data = appSettings[key].ToString()?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    else
                    {
                        Console.WriteLine("Key not found: " + key);
                    }
                }
            }
            return data;
        }


        #endregion
    }

}
