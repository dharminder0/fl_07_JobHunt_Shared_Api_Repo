using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgProfilesRepository:IBaseRepository<OrgProfiles>
    {
        Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId);
    }
}
