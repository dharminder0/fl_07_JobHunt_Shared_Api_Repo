using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IClientsRepository : IBaseRepository<Clients>
    {
        Task<Clients> GetClientsByIdAsync(int id);
        Task<bool> UpsertClientAsync(ClientsRequest request, string clientCode, string uploadedimageUrl, string uploadedUrl);
        Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode);
        Task<Clients> GetClientsByNameAsync(string name);
        Task<bool> DeleteClientsByIdAsync(string orgCode, int id, string clientName);
        Task<Clients> GetClientsByClientCodeAsync(string clientCode);
        Task<PaginationDto<ClientsResponse>> GetClientsListAsync(ClientsSearchRequest request);
    }
}
