using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode);
        Task<string> UpsertAsync(string companyName, string email, string companyCode);
        Task<ActionMessageResponseModel> AddCompanyInformationAsync(CompanyInfoRequestModel companyInfo);
        Task<ActionMessageResponseModel> GetAllCompanyDetailsAsync(string companyCode, List<string> roleType);
        Task<List<CompanyUserListDto>> GetCompanyUserListAsync(string companyCode);
        Task<List<CompanyUserListDto>> GetCompanyUserListByRoleTypeAsync(string companyCode, string roleType);
    }
}
