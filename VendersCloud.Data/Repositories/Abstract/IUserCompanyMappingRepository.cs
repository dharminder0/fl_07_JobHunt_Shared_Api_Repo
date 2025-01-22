using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserCompanyMappingRepository : IDataRepository<UserCompanyMapping>
    {
        Task<UserCompanyMapping> GetMappingsByUserIdAsync(string userId);
        Task<bool> AddMappingAsync(string userId, string companyCode);
        Task<List<UserCompanyMapping>> GetMappingsByCompanyCodeAsync(string companyCode);
    }
}
