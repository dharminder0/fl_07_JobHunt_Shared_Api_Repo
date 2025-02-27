using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class BenchRepository : StaticBaseRepository<Resources>, IBenchRepository
    {
        public BenchRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
