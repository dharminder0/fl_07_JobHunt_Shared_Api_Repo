namespace VendersCloud.Business.Service.Abstract
{
    public interface IClientsService
    {
        Task<Clients> GetClientsByIdAsync(int id);
        Task<ActionMessageResponse> UpsertClientAsync(ClientsRequest request);
        Task<Clients> GetClientsByNameAsync(string name);
        Task<List<ClientDropDownList>> GetClientsByOrgCodeAsync(string orgCode);
        Task<ActionMessageResponse> DeleteClientsByIdAsync(string orgCode, int id, string clientName);
        Task<PaginationDto<ClientsResponse>> GetClientsListAsync(ClientsSearchRequest request);
        Task<Clients> GetClientsByClientCodeAsync(string clientCode);
    }
}
