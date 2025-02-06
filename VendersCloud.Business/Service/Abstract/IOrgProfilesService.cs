namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrgProfilesService
    {
        Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId);
    }
}
