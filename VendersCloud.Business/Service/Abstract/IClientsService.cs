using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IClientsService
    {
        Task<Clients> GetClientsByIdAsync(int id);
        Task<ActionMessageResponse> UpsertClientAsync(ClientsRequest request);
        Task<Clients> GetClientsByNameAsync(string name);
        Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode);
        Task<ActionMessageResponse> DeleteClientsByIdAsync(string orgCode, int id, string clientName);
        Task<PaginationDto<ClientsResponse>> GetClientsListAsync(ClientsSearchRequest request);
    }
}
