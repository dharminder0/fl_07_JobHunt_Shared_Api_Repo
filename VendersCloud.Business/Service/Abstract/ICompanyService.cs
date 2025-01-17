using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode);
        Task<string> UpsertAsync(string companyName, string email, string companyCode);
        Task<ActionMessageResponseModel> AddCompanyInformationAsync(CompanyInfoRequestModel companyInfo);
    }
}
