using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgProfilesRepository:IBaseRepository<OrgProfiles>
    {
        Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId);
        Task<PaginationDto<Organization>> SearchOrganizationsDetails(SearchRequest request);
    }
}
