using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace VendersCloud.Business.Service.Concrete
{
    public class BlobStorageService : IBlobStorageService
    {
        public IConfiguration _configuration;
        private readonly ExternalConfigReader _externalConfig;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _externalConfig = new ExternalConfigReader(configuration);
        }
        public async Task<string> UploadBase64ToBlobAsync(FileRequest fileRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRequest?.FileData))
                    throw new ArgumentException("File data is empty or null");
                var files= Convert.FromBase64String(fileRequest.FileData);
                var filesnames = fileRequest.FileName;
                var fileNames = fileRequest.FileName.Trim('\"');
                fileNames = fileNames.Replace(" ", "").Replace("-", "");

                // Upload to Azure Blob Storage
                var res= await UploadToBlobAsync(files, fileRequest.FileName);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Blob Storage Error: {ex.Message}");
                throw;
            }
        }

       

        private async Task<string> UploadToBlobAsync(byte[] fileBytes, string originalFileName)
        {
            using (MemoryStream stream = new MemoryStream(fileBytes))
            {
                string connectionString = _externalConfig.GetBlobStorageAccount();
                string containerName = _externalConfig.GetBlobContainerName();

                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(originalFileName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }
        }


    }
}
