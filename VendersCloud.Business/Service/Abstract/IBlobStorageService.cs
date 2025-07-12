using Microsoft.AspNetCore.Http;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IBlobStorageService
    {
        Task<string> UploadBase64ToBlobAsync(FileRequest fileRequest);
    }
}
