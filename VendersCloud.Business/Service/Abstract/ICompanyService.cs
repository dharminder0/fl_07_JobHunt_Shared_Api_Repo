using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode);
        Task<string> UpsertAsync(string companyName, string email, string companyCode);
    }
}
