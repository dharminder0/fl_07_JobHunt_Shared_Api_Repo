using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IMasterListRepository : IBaseRepository<MasterList>
    {
        Task<List<MasterList>> GetMasterListAsync();
        Task<bool> AddBulkMasterListAsync(List<string> names);
        Task<MasterList> GetMasterListByIdAndNameAsync(string name);
    }
}
