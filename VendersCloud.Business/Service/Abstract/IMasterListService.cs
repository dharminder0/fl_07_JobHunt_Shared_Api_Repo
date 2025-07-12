using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IMasterListService
    {
        Task<List<MasterList>> GetMasterListAsync();
        Task<bool> AddBulkMasterListAsync(List<string> names);
        Task<MasterList> GetMasterListByIdAndNameAsync(string name);
    }
}
