using Microsoft.AspNetCore.Http;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IBlobStorageService
    {
        Task<string> DownloadAndUploadToBlobAsync(string fileUrl);
    }
}
