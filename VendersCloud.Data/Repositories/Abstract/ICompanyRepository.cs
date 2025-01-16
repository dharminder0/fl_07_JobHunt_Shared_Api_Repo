using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ICompanyRepository : IDataRepository<Company>
    {
        Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode);
        Task<string> UpsertAsync(string companyName, string email, string companyCode);
    }
}
