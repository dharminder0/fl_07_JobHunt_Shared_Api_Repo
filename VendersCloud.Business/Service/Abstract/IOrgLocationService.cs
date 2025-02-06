using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrgLocationService
    {
        Task<bool> UpsertLocation(OrgLocation location);
        Task<List<OrgLocation>> GetOrgLocation(string orgCode);
    }
}
