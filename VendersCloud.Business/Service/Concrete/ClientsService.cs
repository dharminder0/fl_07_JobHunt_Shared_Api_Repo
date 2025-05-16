using Newtonsoft.Json.Serialization;
using VendersCloud.Business.CommonMethods;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class ClientsService :IClientsService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IRequirementVendorsRepository _requirementVendorsRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IPartnerVendorRelRepository _partnerVendorRelRepository;
        public ClientsService(IClientsRepository clientsRepository, IBlobStorageService blobStorageService, IRequirementRepository requirementRepository,
            IRequirementVendorsRepository requirementVendorsRepository, IResourcesRepository resourcesRepository, IPartnerVendorRelRepository partnerVendorRelRepository)
        {
            _clientsRepository = clientsRepository;
            _blobStorageService = blobStorageService;
            _requirementRepository = requirementRepository;
            _requirementVendorsRepository = requirementVendorsRepository;
            _resourcesRepository = resourcesRepository;
            _partnerVendorRelRepository = partnerVendorRelRepository;
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
                        if (!string.IsNullOrEmpty(file.FileName)  || !string.IsNullOrEmpty(file.FileData))
                        {
                            uploadedimageUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);
                        }

                    }
                    
                }

                // Upload Favicon files if provided
                if (request.FaviconURL != null && request.FaviconURL.Count > 0)
                {
                    List<string> uploadedFavicons = new List<string>();
                    foreach (var file in request.FaviconURL)
                    {
                        if (!string.IsNullOrEmpty(file.FileName) || !string.IsNullOrEmpty(file.FileData))
                        {
                            uploadedUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);
                        }
                    }
                }

                // Generate Client Code
                string clientCode = CommonFunctions.GenerateRandomClientCode();
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
                var response = await _clientsRepository.GetClientsListAsync(request);

                foreach (var item in response.List)
                {
               
                 

                    var orgRelationshipData = await _partnerVendorRelRepository.GetBenchResponseListByIdAsync(item.OrgCode);
                    var partnerCodes = orgRelationshipData.Select(v => v.PartnerCode).ToList();

                    var requirementVendorIds = await _requirementVendorsRepository.GetRequirementShareJobsAsync(item.OrgCode);

                
                    var sharedRequirements = await _requirementRepository.GetRequirementByIdAsync(requirementVendorIds);
                    var publicRequirements = await _requirementRepository.GetPublicRequirementAsync(partnerCodes, 3);

              
                    var allRequirements = sharedRequirements.Concat(publicRequirements).ToList();

                    int activeContracts = 0;
                    int pastContracts = 0;
                    int openRequirements = 0;

                    foreach (var req in allRequirements)
                    {
                        openRequirements = await _requirementRepository.GetRequirementCountByOrgCodeAsyncV2(item.OrgCode);
                        var applications = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(req.Id);
                        activeContracts += applications.Count(v => v.Status == (int)RecruitmentStatus.Onboarded);
                        pastContracts += applications.Count(v => v.Status == (int)RecruitmentStatus.ContractClosed);
                    }
                    item.OpenRequirements = openRequirements;
                    item.ActiveContracts = activeContracts;
                    item.PastContracts = pastContracts;
                }

                return response;
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }

    }
}
