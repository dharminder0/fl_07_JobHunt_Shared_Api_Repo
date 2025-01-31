using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class StaticBaseRepository<T> :BaseRepository<T>,IBaseRepository<T> where T : class, IEntityKey, new()
    {
        public StaticBaseRepository(IConfiguration configuration) : base(configuration) { }
    }
    
}
