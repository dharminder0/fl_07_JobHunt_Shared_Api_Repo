namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IBenchRepository :IBaseRepository<Resources>
    {
        Task<bool> UpsertBenchMembersAsync(BenchRequest request);
        Task<List<Resources>> GetBenchResponseListAsync(string orgCode);
        Task<List<Resources>> GetBenchListBySearchAsync(BenchSearchRequest request);
        Task<IEnumerable<Resources>> GetBenchResponseListByIdAsync(List<int> benchId);
    }
}
