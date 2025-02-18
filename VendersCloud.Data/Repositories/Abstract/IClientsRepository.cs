using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IClientsRepository : IBaseRepository<Clients>
    {
        Task<Clients> GetClientsByIdAsync(int id);
        Task<bool> UpsertClientAsync(ClientsRequest request, string clientCode);
        Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode);
        Task<Clients> GetClientsByNameAsync(string name);
        Task<bool> DeleteClientsByIdAsync(string orgCode, int id, string clientName);
        Task<Clients> GetClientsByClientCodeAsync(string clientCode);
    }
}
