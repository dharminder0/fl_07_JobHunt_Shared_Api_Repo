
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserProfilesService
    {
        Task<bool> InsertUserProfileAsync(int userId, int profileId);
        Task<List<UserProfiles>> GetProfileRole(int userId);
    }
}
