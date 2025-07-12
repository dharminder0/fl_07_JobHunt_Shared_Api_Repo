using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IListValuesService
    {
        Task<IList<ListValues>> GetListValuesAsync();
        Task<ListValues> GetListValuesByNameAsync(string Name);
        Task<List<ListValues>> GetListValuesByMasterListIdAsync(string name);
    }
}
