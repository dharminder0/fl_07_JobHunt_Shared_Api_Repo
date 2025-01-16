using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserCompanyMappingRepository : IDataRepository<UserCompanyMapping>
    {
        Task<UserCompanyMapping> GetMappingsByUserIdAsync(string userId);
        Task AddMappingAsync(string userId, string companyCode);
    }
}
