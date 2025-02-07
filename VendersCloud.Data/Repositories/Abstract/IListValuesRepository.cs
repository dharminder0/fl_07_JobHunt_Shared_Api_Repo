using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IListValuesRepository : IBaseRepository<ListValues>
    {
        Task<List<ListValues>> GetListValuesByMasterListIdAsync(int mastervalue);
        Task<ListValues> GetListValuesByNameAsync(string name);
        Task<IList<ListValues>> GetListValuesAsync();
    }
}
