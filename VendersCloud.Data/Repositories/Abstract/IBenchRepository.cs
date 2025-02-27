using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IBenchRepository :IBaseRepository<Resources>
    {
        Task<bool> UpsertBenchMembersAsync(BenchRequest request);
        Task<List<Resources>> GetBenchResponseListAsync(string orgCode);
    }
}
