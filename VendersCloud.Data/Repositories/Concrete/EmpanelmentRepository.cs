using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class EmpanelmentRepository : StaticBaseRepository<Empanelment>, IEmpanelmentRepository
    {
        public EmpanelmentRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
