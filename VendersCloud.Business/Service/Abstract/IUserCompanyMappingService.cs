using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserCompanyMappingService
    {
        Task<UserCompanyMapping> GetMappingsByUserIdAsync(string userId);
        Task<bool> AddMappingAsync(string userId, string companyCode);
    }
}
