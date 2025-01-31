using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementRepository : StaticBaseRepository<Requirement>,IRequirementRepository
    {
        public RequirementRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
