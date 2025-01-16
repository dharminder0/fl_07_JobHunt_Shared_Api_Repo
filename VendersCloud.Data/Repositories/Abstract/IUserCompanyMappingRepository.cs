using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserCompanyMappingRepository : IDataRepository<UserCompanyMapping>
    {
        Task<UserCompanyMapping> GetMappingsByUserId(string userId);
        Task AddMapping(string userId, string companyCode);
    }
}
