using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgSocialRepository:StaticBaseRepository<OrgSocial>,IOrgSocialRepository
    {
        public OrgSocialRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
