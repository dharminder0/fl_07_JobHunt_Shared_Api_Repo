using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgProfilesRepository:StaticBaseRepository<OrgProfiles>, IOrgProfilesRepository
    {
        public OrgProfilesRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
