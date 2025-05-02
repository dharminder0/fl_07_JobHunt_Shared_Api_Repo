using Azure.Core;
using Newtonsoft.Json;
using System.Dynamic;
using VendersCloud.Business.CommonMethods;

namespace VendersCloud.Business.Service.Concrete
{
    public class BenchService : IBenchService
    {
        private readonly IBenchRepository _benchRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IRequirementRepository _requirementsRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IOrgRelationshipsRepository _orgRelationshipsRepository;
        private readonly IUsersRepository _userRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ISkillResourcesMappingRepository _skillRequirementMappingRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMatchRecordRepository _matchRecordRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly IPartnerVendorRelRepository _partnerVendorRelRepository;
        public BenchService(IBenchRepository benchRepository, IResourcesRepository resourcesRepository, IRequirementRepository requirementsRepository,
            IOrganizationRepository organizationRepository, 
            IClientsRepository clientsRepository, IOrgRelationshipsRepository orgRelationshipsRepository, IUsersRepository _usersRepository,
            ISkillRepository skillRepository, ISkillResourcesMappingRepository skillRequirementMappingRepository, IBlobStorageService blobStorageService,
            IMatchRecordRepository matchRecordRepository, IRequirementRepository requirementRepository, IPartnerVendorRelRepository partnerVendorRelRepository)
        {
            _benchRepository = benchRepository;
            _resourcesRepository = resourcesRepository;
            _requirementsRepository = requirementsRepository;
            _organizationRepository = organizationRepository;
            _clientsRepository = clientsRepository;
            _orgRelationshipsRepository = orgRelationshipsRepository;
            _userRepository = _usersRepository;
            _skillRepository = skillRepository;
            _skillRequirementMappingRepository = skillRequirementMappingRepository;
            _blobStorageService = blobStorageService;
            _matchRecordRepository = matchRecordRepository;
             _requirementRepository = requirementRepository;
            _partnerVendorRelRepository = partnerVendorRelRepository;
        }

        public async Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(benchRequest.OrgCode) || string.IsNullOrEmpty(benchRequest.FirstName))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Enter Valid Inputs",
                        Content = ""
                    };
                }
                var res = await _benchRepository.UpsertBenchMembersAsync(benchRequest);
                if (res > 0)
                    if (benchRequest.cv.top_skills != null)
                    {
                        var data = await _skillRepository.SkillUpsertAsync(benchRequest.cv.top_skills);
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                await _skillRequirementMappingRepository.UpsertSkillRequirementMappingAsync(item.Id, res);
                            }
                            return new ActionMessageResponse()
                            {
                                Success = true,
                                Message = "Bench Member added",
                                Content = res
                            };
                        }
                    }
              
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = "Bench Member not added",
                    Content = ""
                };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Content = ""
                };
            }
        }

        public async Task<List<Resources>> GetBenchListAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgCode))
                {
                    throw new Exception("Enter Valid Input");
                }
                return await _benchRepository.GetBenchResponseListAsync(orgCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationDto<BenchResponse>> GetBenchListBySearchAsync(BenchSearchRequest request)
        {
            if (string.IsNullOrEmpty(request.OrgCode))
            {
                throw new ArgumentException("Enter Valid Inputs");
            }

            try
            {
                var response = await _benchRepository.GetBenchListBySearchAsync(request);
                var totalRecords = response.Count;
                var paginatedResponse = response.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                var benchResponseList = new List<BenchResponse>();

                foreach (var item in paginatedResponse)
                {
                    var matchResultList = await GetBenchMatchResultAsync(new BenchMatchRecord
                    {
                        ResourcesId = item.Id,
                        OrgCode = item.OrgCode
                    });
                    int totalMatchCount = matchResultList.Sum(x => (int)x.MatchingRecordCount);
                    var benchResponse = new BenchResponse
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Title = item.Title,
                        Email = item.Email,
                        CV = await GetCvByIdAsync(item.Id),
                        Avtar = item.Avtar,
                        OrgCode = item.OrgCode,
                        Availability = item.Availability,
                        AvailabilityName = CommonFunctions.GetEnumDescription((BenchAvailability)item.Availability),
                        CreatedOn = item.CreatedOn,
                        UpdatedOn = item.UpdatedOn,
                        CreatedBy = item.CreatedBy,
                        UpdatedBy = item.UpdatedBy,
                        IsDeleted = item.IsDeleted,
                        MatchingCount = totalMatchCount
                    };

                    benchResponseList.Add(benchResponse);
                }

                return new PaginationDto<BenchResponse>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    List = benchResponseList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ActionMessageResponse> UpsertApplicants(ApplicationsRequest request)
        {
            try
            {
                if (request.ResourceId == null || !request.ResourceId.Any() || string.IsNullOrEmpty(request.RequirementUniqueId))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Invalid input: ResourceId cannot be null or empty and RequirementId must be positive.",
                        Content = ""
                    };
                }
                int Id = 0;
                var requirementdata = await _requirementsRepository.GetRequirementListByIdAsync(request.RequirementUniqueId);
                if (requirementdata != null)
                {
                    foreach (var requirement in requirementdata)
                    {
                        Id = requirement.Id;
                    }
                }
                var res = await _resourcesRepository.UpsertApplicants(request, Id);
                if (res)
                {
                    return new ActionMessageResponse()
                    {
                        Success = true,
                        Message = "Applicants have been successfully added!",
                        Content = ""
                    };
                }

                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = "Failed to add applicants. Please try again.",
                    Content = ""
                };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Content = ""
                };
            }
        }

        public async Task<PaginationDto<ApplicantsSearchResponse>> GetSearchApplicantsList(ApplicantsSearchRequest request)
        {
            try
            {
                List<ApplicantsSearchResponse> listSearchResponse = new List<ApplicantsSearchResponse>();

                var applications = await _resourcesRepository.GetApplicationsList();
                var query = applications.AsQueryable();
                var clientsList = new List<Clients>();
                var clientsData = new Dictionary<string, Clients>();
                if (!string.IsNullOrEmpty(request.UserId) && int.TryParse(request.UserId, out var id))
                {
                    query = query.Where(a => a.CreatedBy == id);
                }

                if (request.Status != null && request.Status.Any())
                {
                    query = query.Where(a => request.Status.Contains(a.Status));
                }

                var resourceIds = query.Select(a => a.ResourceId).Distinct().ToList();
                var benchDataList = await _benchRepository.GetBenchResponseListByIdAsync(resourceIds);
                var benchData = benchDataList.GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.ToList());

                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    query = query.Where(a =>
                        benchData.ContainsKey(a.ResourceId) &&
                        benchData[a.ResourceId].Any(r =>
                            (!string.IsNullOrEmpty(r.FirstName) && r.FirstName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(r.LastName) && r.LastName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
                        )
                    );
                }

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var pagedResults = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

                var requirementIds = pagedResults.Select(a => a.RequirementId).Distinct().ToList();
                var requirementsList = await _requirementsRepository.GetRequirementByIdAsync(requirementIds);
                var requirementsData = requirementsList.ToDictionary(r => r.Id, r => r);

                var clientCodes = requirementsList.Select(r => r.ClientCode).Where(code => !string.IsNullOrWhiteSpace(code)).Distinct().ToList();
                if (clientCodes.Any())
                {
                    clientsList = await _clientsRepository.GetClientsByClientCodeListAsync(clientCodes);
                    clientsData = clientsList.ToDictionary(c => c.ClientCode, c => c);
                }
                foreach (var data in pagedResults)
                {
                    var searchResponse = new ApplicantsSearchResponse
                    {
                        Status = data.Status,
                        StatusName = CommonFunctions.GetEnumDescription((RecruitmentStatus)data.Status),
                        Comment = data.Comment,
                        ApplicationDate = data.CreatedOn
                    };

                    if (requirementsData.TryGetValue(data.RequirementId, out var requirement))
                    {
                        searchResponse.Title = requirement.Title;
                        searchResponse.Id = data.Id;
                        searchResponse.CV = await GetCvByIdAsync(data.Id);
                        searchResponse.UniqueId = requirement.UniqueId;
                        var matchScoreResult = await _matchRecordRepository.GetMatchScoreAsync(data.RequirementId, data.ResourceId);
                        searchResponse.MatchScore = matchScoreResult.MatchScore;
                        if (clientCodes.Count != 0)
                        {
                            if (!string.IsNullOrEmpty(requirement.ClientCode))
                            {
                                if (clientsData.TryGetValue(requirement.ClientCode, out var client))
                                {
                                    if (request.ClientOrgCode == null || !request.ClientOrgCode.Any() || request.ClientOrgCode.Contains(client.ClientCode))
                                    {
                                        searchResponse.ClientOrgName = client.ClientName;
                                        searchResponse.ClientOrgLogo = client.LogoURL;
                                        searchResponse.ClientCode = requirement.ClientCode;
                                    }
                                }
                            }
                        }
                    }


                    if (benchData.TryGetValue(data.ResourceId, out var resourceList))
                    {
                        var resource = resourceList.FirstOrDefault();
                        if (resource != null)
                        {
                            searchResponse.FirstName = resource.FirstName;
                            searchResponse.LastName = resource.LastName;
                        }
                    }

                    listSearchResponse.Add(searchResponse);
                }

                return new PaginationDto<ApplicantsSearchResponse>
                {
                    Count = totalCount,
                    Page = request.Page,
                    TotalPages = totalPages,
                    List = listSearchResponse
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching applicants: {ex.Message}", ex);
            }
        }

        public async Task<PaginationDto<OrgActivePositionsResponse>> GetActiveVacanciesByOrgCodeAsync(CompanyActiveClientResponse request)
        {
            try
            {
                List<OrgActivePositionsResponse> orgActivePositionsResponseList = new List<OrgActivePositionsResponse>();

                var data = await _requirementsRepository.GetActivePositionsByOrgCodeAsync(request.OrgCode, null);
                var totalCount = data.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                if (data != null)
                {
                    var pagedData = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                    foreach (var item in pagedData)
                    {
                        OrgActivePositionsResponse orgActivePositionsResponse = new OrgActivePositionsResponse();
                        orgActivePositionsResponse.ClientCode = item.ClientCode;
                        if (string.IsNullOrEmpty(orgActivePositionsResponse.ClientCode))
                        {
                            continue;
                            orgActivePositionsResponse.ClientCode = "";
                            return new PaginationDto<OrgActivePositionsResponse>
                            {
                                Count = totalCount,
                                Page = request.PageNumber,
                                TotalPages = totalPages,
                                List = orgActivePositionsResponseList
                            };
                        }
                        orgActivePositionsResponse.TotalPositions = item.TotalPositions;

                        var clientData = await _clientsRepository.GetClientsByClientCodeAsync(orgActivePositionsResponse.ClientCode);
                        if (clientData != null)
                        {
                            orgActivePositionsResponse.ClientName = clientData.ClientName;
                            orgActivePositionsResponse.ClientFavicon = clientData.FaviconURL;
                        }
                        else
                        {
                            return new PaginationDto<OrgActivePositionsResponse>
                            {
                                Count = totalCount,
                                Page = request.PageNumber,
                                TotalPages = totalPages,
                                List = orgActivePositionsResponseList
                            };
                        }
                        orgActivePositionsResponseList.Add(orgActivePositionsResponse);
                    }
                }
                return new PaginationDto<OrgActivePositionsResponse>
                {
                    Count = totalCount,
                    Page = request.PageNumber,
                    TotalPages = totalPages,
                    List = orgActivePositionsResponseList
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationDto<dynamic>> GetTopVendorsListAsync(CompanyActiveClientResponse request)
        {
            try
            {
                List<dynamic> data = new List<dynamic>();
                var orgrelationshipdata = await _partnerVendorRelRepository.GetBenchResponseListByIdAsync(request.OrgCode);

                // Get related organization codes
                var relatedOrgCodes = orgrelationshipdata
                    .Where(x => x.PartnerCode == request.OrgCode)
                    .Select(x => x.VendorCode)
                    .ToList();

                if (relatedOrgCodes.Any())
                {
                    foreach (var orgCode in relatedOrgCodes)
                    {
                        await ProcessVendorData(orgCode, data);
                    }
                }
                else
                {
                    var orgCodes = orgrelationshipdata
                        .Where(x => x.PartnerCode == request.OrgCode)
                        .Select(x => x.VendorCode)
                        .ToList();

                    foreach (var orgCode in orgCodes)
                    {
                        await ProcessVendorData(orgCode, data);
                    }
                }
                data = data.OrderByDescending(x => x.TotalPlacements).ToList();
                // Pagination logic
                var totalRecords = data.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);
                return new PaginationDto<dynamic>
                {
                    Count = totalRecords,
                    Page = request.PageNumber,
                    TotalPages = totalPages,
                    List = data
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ProcessVendorData(string orgCode, List<dynamic> data)
        {
            var userlist = await _userRepository.GetUserByOrgCodeAsync(orgCode);
            var organization = await _organizationRepository.GetOrganizationData(orgCode);

            if (userlist != null && userlist.Any())
            {
                var userIds = userlist.Select(x => x.Id).ToList();
                var applicationsData = await _resourcesRepository.GetTotalPlacementsByUserIdsAsync(userIds);

                var vendorInfo = new
                {
                    VendorName = organization?.OrgName,
                    VendorLogo = organization?.Logo,
                    TotalPlacements = applicationsData
                };

                data.Add(vendorInfo);
            }
        }


        public async Task<PaginationDto<OrgActivePositionsResponse>> GetActiveVacanciesByUserIdAsync(VendorActiveClientResponse request)
        {
            try
            {
                List<OrgActivePositionsResponse> orgActivePositionsResponseList = new List<OrgActivePositionsResponse>();

                var data = await _requirementsRepository.GetActivePositionsByOrgCodeAsync(null, request.UserId);
                var totalCount = data.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                if (data != null)
                {
                    var pagedData = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                    foreach (var item in pagedData)
                    {
                        OrgActivePositionsResponse orgActivePositionsResponse = new OrgActivePositionsResponse();
                        orgActivePositionsResponse.ClientCode = item.ClientCode;
                        orgActivePositionsResponse.TotalPositions = item.TotalPositions;

                        var clientData = await _clientsRepository.GetClientsByClientCodeAsync(item.ClientCode);
                        if (clientData != null)
                        {
                            orgActivePositionsResponse.ClientName = clientData.ClientName;
                            orgActivePositionsResponse.ClientFavicon = clientData.FaviconURL;
                        }
                        else
                        {
                            return new PaginationDto<OrgActivePositionsResponse>
                            {
                                Count = totalCount,
                                Page = request.PageNumber,
                                TotalPages = totalPages,
                                List = orgActivePositionsResponseList
                            };
                        }

                        orgActivePositionsResponseList.Add(orgActivePositionsResponse);
                    }
                }
                return new PaginationDto<OrgActivePositionsResponse>
                {
                    Count = totalCount,
                    Page = request.PageNumber,
                    TotalPages = totalPages,
                    List = orgActivePositionsResponseList
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionMessageResponse> GetVendorContractsAsync(VendorContractRequest request)
        {
            try
            {


                var records = await _resourcesRepository.GetContractsByTypeAsync(request);

                return new ActionMessageResponse
                {
                    Success = true,
                    Message = "Contracts fetched successfully.",
                    Content = new VendorContractResponse
                    {
                   
                        Records = records.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList(),
                        TotalRecords = records.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Content = ""
                };
            }
        }
        public async Task<dynamic> GetCountTechStackByOrgCodeAsync(string orgCode)
        {
            try
            {
                return await _requirementsRepository.GetCountTechStackByOrgCodeAsync(orgCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetCvByIdAsync(int id)
        {
            try
            {
                var data = await _benchRepository.GetBenchResponseByIdAsync(id);
                var cv = data.FirstOrDefault();

                if (cv == null)
                    return null;

                var avatar = await GetAvtarByIdAsync(id);
                var cvJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(cv.CV.ToString());
                cvJson["avatar"] = avatar;

                return cvJson;
            }
            catch (Exception ex)
            {
                throw; 
            }
        }


        public async Task<bool> UpsertCvAvtarAsync(UpsertCvAvtarRequest request)
        {
            try
            {
                string uploadedimageUrl = string.Empty;
                // Upload Logo files if provided
                if (request.LogoURL != null && request.LogoURL.Count > 0)
                {
                    List<string> uploadedLogos = new List<string>();
                    foreach (var file in request.LogoURL)
                    {
                        if (!string.IsNullOrEmpty(file.FileName) || !string.IsNullOrEmpty(file.FileData))
                        {
                            uploadedimageUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);

                        }

                    }
                    bool result = await _benchRepository.UpsertAvtarbyIdAsync(request.BenchId, uploadedimageUrl);
                    return result;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetAvtarByIdAsync(int benchId)
        {
            try
            {
                var link= await _benchRepository.GetAvtarByIdAsync(benchId);
                return link.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<dynamic>> GetBenchMatchResultAsync(BenchMatchRecord request)
        {
            if (request.ResourcesId <= 0 || string.IsNullOrEmpty(request.OrgCode))
            {
                throw new ArgumentException("Enter valid inputs");
            }

            List<dynamic> resultList = new List<dynamic>();
            var data = await _matchRecordRepository.GetMatchRecordByResourceIdAsync(request.ResourcesId);

            if (data != null)
            {
                foreach (var item in data)
                {
                    var requirement = await _requirementRepository.GetRequirementByRequirementIdAsync(item.RequirementId);
                    if (requirement == null) continue;

                    dynamic obj = new ExpandoObject();
                    obj.RequirementId = item.RequirementId;
                    obj.MatchingScore = item.MatchScore;
                    obj.Title = requirement.Title;
                    obj.RequirementOrgCode = requirement.OrgCode;
                    obj.Description = requirement.Description;
                    obj.Duration = requirement.Duration;
                    obj.Experience = requirement.Experience;
                    obj.Hot = requirement.Hot;
                    obj.IsDeleted = requirement.IsDeleted;
                    obj.Location = requirement.Location;
                    obj.Positions = requirement.Positions;
                    obj.Remarks = requirement.Remarks;
                    obj.UniqueId = requirement.UniqueId;
                    obj.Visibility = requirement.Visibility;
                    obj.UpdatedBy = requirement.UpdatedBy;
                    obj.Status = requirement.Status;
                    obj.UpdatedOn = requirement.UpdatedOn;
                    obj.ClientCode = requirement.ClientCode;
                    obj.CreatedOn = requirement.CreatedOn;
                    var orgData = await _clientsRepository.GetClientsByClientCodeAsync(requirement.ClientCode);
                    if (orgData != null)
                    {
                        obj.ClientName = orgData.ClientName;
                        obj.ClientLogo = orgData.LogoURL;
                    }
                    else
                    {
                        var clientData = await _organizationRepository.GetOrganizationData(requirement.ClientCode);
                        if (clientData != null)
                        {
                            obj.ClientName = clientData.OrgName;
                            obj.ClientLogo = clientData.Logo;
                        }
                    }
                    if (int.TryParse(Convert.ToString(requirement.Location), out int locationValue))
                    {
                        obj.LocationTypeName = Enum.GetName(typeof(LocationType), locationValue);
                    }

                    if (int.TryParse(Convert.ToString(requirement.Visibility), out int visibilityValue))
                    {
                        obj.VisibilityName = Enum.GetName(typeof(Visibility), visibilityValue);
                    }

                    if (int.TryParse(Convert.ToString(requirement.Status), out int statusValue))
                    {
                        obj.StatusName = Enum.GetName(typeof(RequirementsStatus), statusValue);
                    }

                    resultList.Add(obj);
                }
            }

            var filteredList = resultList
                .Where(x => x.Visibility == 3 || x.RequirementOrgCode == request.OrgCode)
                .ToList();

            dynamic result = new ExpandoObject();
            result.MatchingRecordCount = filteredList.Count;
            result.Records = filteredList;

            return new List<dynamic> { result };
        }

    }
}

