using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrganizationRepository:IBaseRepository<Organization>
    {
        Task<string> RegisterNewOrganizationAsync(RegistrationRequest request, string OrgCode);
        Task<Organization> GetOrganizationData(string orgCode);
        Task<List<Organization>> GetOrganizationListAsync();
        Task<bool> UpdateOrganizationByOrgCodeAsync(CompanyInfoRequest infoRequest, string orgCode);
        Task<Users> GetUserByIdAsync(int Id);
        Task<bool> UpdateOrganizationAddressByOrgCodeAsync(string regAddress, string orgCode);
        Task<Organization> GetOrganizationByEmailAndOrgCodeAsync(string email, string orgCode);
        Task<Organization> GetOrganizationDataByIdAsync(int Id);
    }
}
