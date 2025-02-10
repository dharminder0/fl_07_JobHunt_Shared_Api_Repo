using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrgProfilesService
    {
        Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId);
        Task<PaginationDto<Organization>> SearchOrganizationsDetails(SearchRequest request);
    }
}
