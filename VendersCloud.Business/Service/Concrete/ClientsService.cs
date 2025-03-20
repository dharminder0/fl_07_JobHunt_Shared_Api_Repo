using System.Text;

namespace VendersCloud.Business.Service.Concrete
{
    public class ClientsService :IClientsService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IBlobStorageService _blobStorageService;
        public ClientsService(IClientsRepository clientsRepository, IBlobStorageService blobStorageService)
        {
            _clientsRepository = clientsRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<ActionMessageResponse> UpsertClientAsync(ClientsRequest request)
        {
            try
            {
                string uploadedimageUrl = string.Empty;
                string uploadedUrl = string.Empty;
                if (request == null || string.IsNullOrEmpty(request.OrgCode))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter valid input", Content = "" };
                }

                // Upload Logo files if provided
                if (request.LogoURL != null && request.LogoURL.Count > 0)
                {
                    List<string> uploadedLogos = new List<string>();
                    foreach (var file in request.LogoURL)
                    {
                         uploadedimageUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);
                        
                    }
                    
                }

                // Upload Favicon files if provided
                if (request.FaviconURL != null && request.FaviconURL.Count > 0)
                {
                    List<string> uploadedFavicons = new List<string>();
                    foreach (var file in request.FaviconURL)
                    {
                         uploadedUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);
                    }
                }

                // Generate Client Code
                string clientCode = GenerateRandomClientCode();
                bool result = await _clientsRepository.UpsertClientAsync(request, clientCode, uploadedimageUrl, uploadedUrl);

                if (result)
                    return new ActionMessageResponse() { Success = true, Message = "Client Added/Updated", Content = "" };

                return new ActionMessageResponse() { Success = false, Message = "Client Not Added", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
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

        public async Task<Clients> GetClientsByClientCodeAsync(string clientCode)
        {
            try
            {
                if (string.IsNullOrEmpty(clientCode))
                {
                    throw new ArgumentNullException("ClientCode can't be null");
                }
                var response = await _clientsRepository.GetClientsByClientCodeAsync(clientCode);
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

        public async Task<List<ClientDropDownList>> GetClientsByOrgCodeAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    throw new ArgumentNullException("OrgCode can't be null");
                }

                List<ClientDropDownList> dropList = new List<ClientDropDownList>();

                var response = await _clientsRepository.GetClientsByOrgCodeAsync(orgCode);
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var list = new ClientDropDownList
                        {
                            Id = item.Id,
                            Name = item.ClientName,
                            Value = item.ClientCode,
                            Logo= item.LogoURL
                        };

                        dropList.Add(list);
                    }
                }

                return dropList;
            }
            catch (Exception ex)
            {
                throw new Exception( ex.Message);
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

        public async Task<PaginationDto<ClientsResponse>> GetClientsListAsync(ClientsSearchRequest request)
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
