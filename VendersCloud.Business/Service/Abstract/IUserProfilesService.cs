
namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserProfilesService
    {
        Task<bool> UpsertUserProfileAsync(int userId, int profileId);
        Task<int> GetProfileRole(int userId);
    }
}
