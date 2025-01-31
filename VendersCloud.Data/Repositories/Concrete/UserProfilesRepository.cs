using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserProfilesRepository:StaticBaseRepository<UserProfiles>, IUserProfilesRepository
    {
        public UserProfilesRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
