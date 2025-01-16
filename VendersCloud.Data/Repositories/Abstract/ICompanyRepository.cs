using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ICompanyRepository : IDataRepository<Company>
    {
        Task<Company> GetCompanyDetailByCompanyCode(string companyCode);
        Task<string> Upsert(string companyName, string email, string companyCode);
    }
}
