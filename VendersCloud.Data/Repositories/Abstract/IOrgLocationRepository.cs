namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgLocationRepository:IBaseRepository<OrgLocation>
    {
        Task<bool> UpsertLocation(OrgLocation location);
        Task<List<OrgLocation>> GetOrgLocation(string orgCode);
        Task<bool> DeleteOrgLocationAsync(string orgCode);
    }
}
