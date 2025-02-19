using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class BlobStorageService: IBlobStorageService
    {
        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;
        public IConfiguration _configuration;
        private readonly ExternalConfigReader _externalConfig;
        
        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _externalConfig = new ExternalConfigReader(configuration);
            _blobConnectionString = _externalConfig.GetBlobStorageAccount();
            _blobContainerName = _externalConfig.GetBlobContainerName();
        }

        public async Task<string> DownloadAndUploadToBlobAsync(string fileUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(fileUrl);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download image from URL: {fileUrl}");

                    using (Stream fileStream = await response.Content.ReadAsStreamAsync())
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(new Uri(fileUrl).AbsolutePath);
                        return await UploadStreamToBlobAsync(fileStream, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
                return fileUrl; // Fallback: Keep the original URL if upload fails
            }
        }

        private async Task<string> UploadStreamToBlobAsync(Stream fileStream, string fileName)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(_blobConnectionString, _blobContainerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); // Ensure container exists

                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream, overwrite: true);

                return blobClient.Uri.ToString(); // Return public Blob URL
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Blob Storage Error: {ex.Message}");
                throw;
            }
        }
    }
}
