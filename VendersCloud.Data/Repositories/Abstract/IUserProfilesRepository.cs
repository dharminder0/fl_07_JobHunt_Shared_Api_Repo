using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserProfilesRepository:IBaseRepository<UserProfiles>
    {
        Task<bool> InsertUserProfileAsync(int userId, int profileId);
        Task<int> GetProfileRole(int userId);
    }
}
