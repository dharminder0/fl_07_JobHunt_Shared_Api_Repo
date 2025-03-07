namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserProfilesRepository:IBaseRepository<UserProfiles>
    {
        Task<bool> InsertUserProfileAsync(int userId, int profileId);
        Task<List<UserProfiles>> GetProfileRole(int userId);
    }
}
