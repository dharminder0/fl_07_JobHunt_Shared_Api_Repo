using Microsoft.AspNetCore.Mvc;
using System.Text;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class ClientsService :IClientsService
    {
        private readonly IClientsRepository _clientsRepository;
        public ClientsService(IClientsRepository clientsRepository)
        {
            _clientsRepository = clientsRepository;

        }

        public async Task<ActionMessageResponse> UpsertClientAsync(ClientsRequest request)
        {
            try
            {
                if(request==null|| string.IsNullOrEmpty(request.OrgCode))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter Valids Input", Content = "" };
                    
                }
                
                string ClientCode = GenerateRandomClientCode();
                var res= await _clientsRepository.UpsertClientAsync(request,ClientCode);
                if (res)
                    return new ActionMessageResponse() { Success = true, Message = "Client Added/Updated", Content = "" };
                return new ActionMessageResponse() { Success = false, Message = "Client Not Added", Content = "" };
            }
            catch (Exception ex) {
                 return new ActionMessageResponse { Success=false, Message=ex.Message,Content=""};
            }
        }


        public async Task<Clients> GetClientsByIdAsync(int id)
        {
            try
            {
                if(id<=0)
                {
                    throw new ArgumentNullException("Id can't be null");
                }
                var response = await _clientsRepository.GetClientsByIdAsync(id);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Clients> GetClientsByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("ClientName can't be null");
                }
                var response = await _clientsRepository.GetClientsByNameAsync(name);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    throw new ArgumentNullException("orgCode can't be null");
                }
                var response = await _clientsRepository.GetClientsByOrgCodeAsync(orgCode);
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionMessageResponse> DeleteClientsByIdAsync(string orgCode, int id, string clientName)
        {
            try
            {
                if ( string.IsNullOrEmpty(orgCode)|| string.IsNullOrEmpty(clientName)||id<=0)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter Valids Input", Content = "" };
                }
                var res = await _clientsRepository.DeleteClientsByIdAsync(orgCode, id, clientName);
                if(res)
                    return new ActionMessageResponse() { Success = true, Message = "Deleted Successfully", Content = "" };
                return new ActionMessageResponse() { Success = false, Message = "Not Deleted!!", Content = "" };
            }
            catch(Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<PaginationDto<Clients>> GetClientsListAsync(ClientsSearchRequest request)
        {
            try
            {
                return await _clientsRepository.GetClientsListAsync(request);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public string GenerateRandomClientCode()
        {
            Random _random = new Random();
            int length = 10;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[_random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
