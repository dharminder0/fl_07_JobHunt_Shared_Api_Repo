
namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserProfilesService
    {
        Task<bool> InsertUserProfileAsync(int userId, int profileId);
        Task<int> GetProfileRole(int userId);
    }
}
