
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserProfilesService
    {
        Task<bool> InsertUserProfileAsync(int userId, int profileId);
        Task<List<UserProfileDto>> GetProfileRole(int userId);
    }
}
