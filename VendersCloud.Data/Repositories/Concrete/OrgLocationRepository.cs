using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgLocationRepository:StaticBaseRepository<OrgLocation>,IOrgLocationRepository
    {
        public OrgLocationRepository(IConfiguration configuration):base(configuration) 
        {

        }
    }
}
