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

        public async Task<List<UserProfileDto>> GetProfileRole(int userId)
        {
            try
            {
                var response = await _userProfilesRepository.GetProfileRole(userId);

                var userProfileRoles = response.Select(role => new UserProfileDto
                {
                    Id = role.Id,
                    UserId = role.UserId,
                    ProfileId = role.ProfileId,
                    RoleName = Enum.GetName(typeof(RoleType), role.ProfileId),
                    IsDeleted = role.IsDeleted
                }).ToList();
                return userProfileRoles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
