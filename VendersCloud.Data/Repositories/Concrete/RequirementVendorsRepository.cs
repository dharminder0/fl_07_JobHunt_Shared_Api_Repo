using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementVendorsRepository:StaticBaseRepository<RequirementVendors>, IRequirementVendorsRepository
    {
        public RequirementVendorsRepository(IConfiguration configuration):base(configuration)
        {

        }
    }
}
