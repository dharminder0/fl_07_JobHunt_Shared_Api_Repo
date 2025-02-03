using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class UserProfilesService: IUserProfilesService
    {
        private readonly IUserProfilesRepository _userProfilesRepository;

        public UserProfilesService(IUserProfilesRepository userProfilesRepository)
        {
            _userProfilesRepository= userProfilesRepository;
        }

        public async Task<bool> InsertUserProfileAsync(int userId, int profileId)
        {
            try
            {
                if (userId < 0 || profileId < 0)
                {
                    return false;
                }
                var response = await _userProfilesRepository.InsertUserProfileAsync(userId, profileId);
                return response;
            }
            catch (Exception ex) { 
                return false;
            }

        }

        public async Task<List<UserProfiles>> GetProfileRole(int userId)
        {
            try
            {
                var response = await _userProfilesRepository.GetProfileRole(userId);
                return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
