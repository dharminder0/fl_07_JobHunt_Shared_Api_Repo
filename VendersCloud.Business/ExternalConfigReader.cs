using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace VendersCloud.Business
{
    public   class ExternalConfigReader
    {
        private readonly IConfiguration _configuration;

        public ExternalConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Method to get API Key with fallback to external file if not available in appsettings.json
        public string GetApiKey()
        {
            string apiKey = _configuration["OpenAI:ApiKey"];

            // Check if the ApiKey is available in appsettings.json
            if (!string.IsNullOrEmpty(apiKey))
            {
                return apiKey;
            }

            // If ApiKey is not available in appsettings.json, read from external file
            return GetValueFromExternalFile("ApiKey");
        }

        // Method to get Base URL with fallback to external file if not available in appsettings.json
        public string GetBaseUrl()
        {
            string baseUrl = _configuration["OpenAI:BaseUrl"];

            // Check if the BaseUrl is available in appsettings.json
            if (!string.IsNullOrEmpty(baseUrl))
            {
                return baseUrl;
            }

            // If BaseUrl is not available in appsettings.json, read from external file
            return GetValueFromExternalFile("BaseUrl");
        }

        // Helper method to get values from the external file
        private string GetValueFromExternalFile(string key)
        {
            string filePath = _configuration["FilePath"];

            // Ensure the file path is valid
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"The external settings file '{filePath}' does not exist or is invalid.");
            }

            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);

            // Assuming "OpenAI" section exists in the external file
            var value = jsonObject["OpenAI"]?[key]?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The '{key}' is not found in the external settings.");
            }

            return value;
        }
    }

}
