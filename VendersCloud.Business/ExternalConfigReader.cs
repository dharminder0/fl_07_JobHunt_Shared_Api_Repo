namespace VendersCloud.Business
{
    public class ExternalConfigReader

    {
        private readonly IConfiguration _configuration;

        public ExternalConfigReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetApiKey()
        {
            string apiKey = _configuration["AzureOpenAI:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = _configuration["OpenAI:ApiKey"];
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = GetValueFromExternalFile("ApiKey");
            }

            return apiKey;
        }

        public string GetBaseUrl()
        {
            string baseUrl = _configuration["AzureOpenAI:BaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = _configuration["OpenAI:BaseUrl"];
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = GetValueFromExternalFile("BaseUrl");
            }

            return baseUrl;
        }

        public string GetModel()
        {
            string model = _configuration["OpenAI:Model"];

            if (string.IsNullOrEmpty(model))
            {
                model = GetValueFromExternalFile("Model");
            }

            return model;
        }

        public string GetDeploymentId()
        {
            string deploymentId = _configuration["AzureOpenAI:DeploymentId"];

            if (string.IsNullOrEmpty(deploymentId))
            {
                deploymentId = GetValueFromExternalFile("DeploymentId");
            }

            return deploymentId;
        }
        public string GetApiVersion()
        {
            string apiVersion = _configuration["AzureOpenAI:ApiVersion"];

            if (string.IsNullOrEmpty(apiVersion))
            {
                apiVersion = GetValueFromExternalFile("ApiVersion");
            }

            return apiVersion;
        }


        public string GetValueFromExternalFile(string key)
        {
            string filePath = _configuration["FilePath"];

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"The external settings file '{filePath}' does not exist or is invalid.");
            }

            var jsonContent = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonContent);

            // Prioritize AzureOpenAI
            var value = jsonObject["AzureOpenAI"]?[key]?.ToString();

            if (string.IsNullOrEmpty(value))
            {
                value = jsonObject["OpenAI"]?[key]?.ToString();
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The key '{key}' was not found in either OpenAI or AzureOpenAI sections in the external settings file.");
            }

            return value;
        }




        // Method to get Base URL with fallback to external file if not available in appsettings.json
        public string GetSmtpServerdomain()
        {
            string SmtpServerdomain = _configuration["SmtpServerDomain"];

            if (!string.IsNullOrEmpty(SmtpServerdomain))
            {
                return SmtpServerdomain;
            }

            return GetValueFromExternalAppSettingsFile("SmtpServerDomain");
        }

      
        public string GetSmptUserName()
        {
            string SmptUserName = _configuration["SmptUserName"];

            if (!string.IsNullOrEmpty(SmptUserName))
            {
                return SmptUserName;
            }

            return GetValueFromExternalAppSettingsFile("SmptUserName");
        }

      
        public string GetSmtpUserPassword()
        {
            string SmtpUserPassword = _configuration["SmtpUserPassword"];

            if (!string.IsNullOrEmpty(SmtpUserPassword))
            {
                return SmtpUserPassword;
            }

            return GetValueFromExternalAppSettingsFile("SmtpUserPassword");
        }
        public string GetSmtpEmailDomain()
        {
            string SmtpEmailDomain = _configuration["SmtpEmailDomain"];

            if (!string.IsNullOrEmpty(SmtpEmailDomain))
            {
                return SmtpEmailDomain;
            }

            return GetValueFromExternalAppSettingsFile("SmtpEmailDomain");
        }

        public string GetVerifyEmailDomainUrl()
        {
            string VerifyEmailDomainUrl = _configuration["WebAppUrl"];

            if (!string.IsNullOrEmpty(VerifyEmailDomainUrl))
            {
                return VerifyEmailDomainUrl;
            }

            return GetValueFromExternalAppSettingsFile("WebAppUrl");
        }

        public string GetBlobContainerName()
        {
            string BlobContainerName = _configuration["BlobContainerName"];

            if (!string.IsNullOrEmpty(BlobContainerName))
            {
                return BlobContainerName;
            }

            return GetValueFromExternalAppSettingsFile("BlobContainerName");
        }
        public string GetBlobStorageAccount()
        {
            string BlobStorageAccount = _configuration["BlobStorageAccount"];

            if (!string.IsNullOrEmpty(BlobStorageAccount))
            {
                return BlobStorageAccount;
            }

            return GetValueFromExternalAppSettingsFile("BlobStorageAccount");
        }

        public string GetAuthorizationBearer()
        {
            string AuthorizationBearer = _configuration["AuthorizationBearer"];

            if (!string.IsNullOrEmpty(AuthorizationBearer))
            {
                return AuthorizationBearer;
            }

            return GetValueFromExternalAppSettingsFile("AuthorizationBearer");
        }
        public string GetBlobSymmetricSecretKey()
        {
            string BlobSymmetricSecretKey = _configuration["BlobSymmetricSecretKey"];

            if (!string.IsNullOrEmpty(BlobSymmetricSecretKey))
            {
                return BlobSymmetricSecretKey;
            }

            return GetValueFromExternalAppSettingsFile("BlobStorageAccount");
        }
        // Helper method to get values from the external file
        private string GetValueFromExternalAppSettingsFile(string key)
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
            var value = jsonObject[key]?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The '{key}' is not found in the external settings.");
            }

            return value;
        }

    }

}
