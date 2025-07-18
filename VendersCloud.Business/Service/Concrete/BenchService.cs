﻿using AutoMapper.Internal;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlKata;
using System.Dynamic;
using VendersCloud.Business.CommonMethods;
using VendersCloud.Business.Entities.Dtos;
using static VendersCloud.Common.Extensions.StringExtensions;

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
        private readonly IRequirementVendorsRepository _requirementVendorsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public BenchService(IBenchRepository benchRepository, IResourcesRepository resourcesRepository, IRequirementRepository requirementsRepository,
            IOrganizationRepository organizationRepository, 
            IClientsRepository clientsRepository, IOrgRelationshipsRepository orgRelationshipsRepository, IUsersRepository _usersRepository,
            ISkillRepository skillRepository, ISkillResourcesMappingRepository skillRequirementMappingRepository, IBlobStorageService blobStorageService,
            IMatchRecordRepository matchRecordRepository, IRequirementRepository requirementRepository, IPartnerVendorRelRepository partnerVendorRelRepository,
            IRequirementVendorsRepository requirementVendorsRepository, INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
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
            _requirementVendorsRepository = requirementVendorsRepository;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
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

                if (request.TopSkillId != 0)
                {
                
                    var skillIds = new List<int> { request.TopSkillId.Value };
                    var skillsName = await _skillRepository.GetAllSkillNamesAsync(skillIds);
                    string skillName = skillsName.FirstOrDefault();

                    if (!string.IsNullOrEmpty(skillName))
                    {
                        benchResponseList = benchResponseList.Where(benchResponse =>
                            benchResponse.CV != null &&
                            benchResponse.CV.TryGetValue("top_skills", out var topSkillsObj) &&
                            topSkillsObj is Newtonsoft.Json.Linq.JArray topSkillsJArray &&
                            topSkillsJArray.Any(skill => skill.ToString().Equals(skillName, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }

                return new PaginationDto<BenchResponse>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(benchResponseList.Count / (double)request.PageSize),
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
                    try
                    {
                        var vendorObj = await _userRepository.GetUserByIdAsync(int.Parse(request.UserId));
                        string vendorName = $"{vendorObj.FirstName} {vendorObj.LastName}";

                        // 1️⃣ Insert notification into DB
                        var orgCode = requirementdata.Select(v => v.OrgCode).First();
                        string message = $"A resource has been applied to your requirement by vendor {vendorName}.";

                        await _notificationRepository.InsertNotificationAsync(
                            orgCode,
                            message,
                            (int)NotificationType.ResourceApplied,
                            "Resource Applied Notification"
                        );

                        // 2️⃣ Send real-time SignalR notification to organization group
                        await _hubContext.Clients.Group(orgCode)
                            .SendAsync("ReceiveNotification", new
                            {
                                OrgCode = orgCode,
                                Message = message,
                                NotificationType = (int)NotificationType.ResourceApplied,
                                CreatedOn = DateTime.UtcNow,
                                title = "Resource Applied Notification"
                            });
                    }
                    catch (Exception ex)
                    {
                        // Log exception if needed, skip crashing
                    }

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
                var orgList = new List<Organization>();
                var clientsData = new Dictionary<string, Organization>();
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
                if (!string.IsNullOrEmpty(request.OrgCode)){
                    benchDataList = benchDataList.Where(v => v.OrgCode == request.OrgCode);

                }
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

                var orgCodes = requirementsList.Select(r => r.OrgCode).Where(code => !string.IsNullOrWhiteSpace(code)).Distinct().ToList();
                if (orgCodes.Any())
                {
                    orgList = await _organizationRepository.GetOrgByListAsync(orgCodes);
                    clientsData = orgList.ToDictionary(c => c.OrgCode, c => c);
                }
                foreach (var data in pagedResults)
                {
                    var searchResponse = new ApplicantsSearchResponse
                    {
                        Status = data.Status,
                        StatusName = CommonFunctions.GetEnumDescription((RecruitmentStatus)data.Status),
                        Comment = data.Comment,
                        ApplicationDate = data.CreatedOn,
                        ApplicationId=data.Id
                        
                    };

                    if (requirementsData.TryGetValue(data.RequirementId, out var requirement))
                    {
                        searchResponse.Title = requirement.Title;
                        searchResponse.Id = data.Id;
                        searchResponse.CV = await GetCvByIdAsync(data.ResourceId);
    //                    searchResponse.Avatar = benchDataList
    //.FirstOrDefault(v => v.Id == data.ResourceId)?.Avtar;





                        searchResponse.UniqueId = requirement.UniqueId;
                        var matchScoreResult = await _matchRecordRepository.GetMatchScoreAsync(data.RequirementId, data.ResourceId);
                        searchResponse.MatchScore = matchScoreResult.MatchScore;
                      
                        if (orgCodes.Count != 0)
                        {
                            if (!string.IsNullOrEmpty(requirement.OrgCode))
                            {
                                if (clientsData.TryGetValue(requirement.OrgCode, out var organization))
                                {
                                    if (request.ClientOrgCode == null || !request.ClientOrgCode.Any() || request.ClientOrgCode.Contains(organization.OrgCode))
                                    {
                                        searchResponse.OrgCode = organization.OrgCode;
                                        searchResponse.OrgName = organization.OrgName;
                                        searchResponse.OrgLogo = organization.Logo;
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
                if (!string.IsNullOrWhiteSpace(request.UniqueId))
                {
                    listSearchResponse = listSearchResponse.Where(v => v.UniqueId == request.UniqueId).ToList();
                }
                return new PaginationDto<ApplicantsSearchResponse>
                {
                    Count = totalCount,
                    Page = request.Page,
                    TotalPages = totalPages,
                    List = listSearchResponse.OrderByDescending(v => v.Id).ToList()
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
                List<OrgActivePositionsResponse> orgActivePositionsResponseList = new();
                var pubicReq = new List<Requirement>();



                List<int> requirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.VendorCode);
                var sharedRequirements = (await _requirementRepository.GetRequirementByIdAsync(requirementVendorsId)).ToList();
                if (sharedRequirements != null && sharedRequirements.Any())
                {
                     pubicReq = await _requirementRepository.GetPublicRequirementAsync(sharedRequirements.Select(v => v.OrgCode).ToList(), 3);
                }
                sharedRequirements = sharedRequirements.Concat(pubicReq).ToList();


                var topOrg = sharedRequirements
         .Where(r => !string.IsNullOrEmpty(r.OrgCode))
         .GroupBy(r => r.OrgCode)
         .Select(g => new
         {
             OrgCode = g.Key,
             Count = g.Select(x => x.Id).Distinct().Count()
         })
         .OrderByDescending(x => x.Count)
         .FirstOrDefault();

                if (topOrg != null)
                {

                    var topOrgRequirements = sharedRequirements
          .Where(r => r.OrgCode == topOrg.OrgCode)
          .GroupBy(r => r.OrgCode) 
          .Select(g => g.First()) 
          .ToList();


                    var totalCount = topOrgRequirements.Count;
                    var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                    var pagedData = topOrgRequirements
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

            
                    foreach (var req in pagedData)
                    {
                        var responseItem = new OrgActivePositionsResponse
                        {
                            ClientCode = req.OrgCode,
                            TotalPositions = sharedRequirements.Sum(v=>v.Positions)
                          
                        };

                        var clientData = await _organizationRepository.GetOrganizationData(req.OrgCode);
                        if (clientData != null)
                        {
                            responseItem.ClientName = clientData.OrgName;
                            responseItem.ClientFavicon = clientData.Logo;
                           
                        }

                        orgActivePositionsResponseList.Add(responseItem);
                    }

                    return new PaginationDto<OrgActivePositionsResponse>
                    {
                        Count = totalCount,
                        Page = request.PageNumber,
                        TotalPages = totalPages,
                        List = orgActivePositionsResponseList
                    };
                }

                // If no top org found
                return new PaginationDto<OrgActivePositionsResponse>
                {
                    Count = 0,
                    Page = request.PageNumber,
                    TotalPages = 0,
                    List = orgActivePositionsResponseList
                };
            }
            catch (Exception ex)
            {
                throw; // Let higher layers handle the exception
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
        public async Task<dynamic> GetCountTechStackByOrgCodeAsync(TechStackRequest request)
        {
            try
            {
                return  await _requirementsRepository.GetCountTechStackByOrgCodeAsync(request);
               
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
        public async Task<bool> UpdateResourceAvailabilityAsync(UpdateAvailabilityRequest request)
        {
            if (request == null || request.Id <= 0 || string.IsNullOrWhiteSpace(request.OrgCode))
            {
                return false;
            }

            return await _benchRepository.UpdateResourceAvailabilityAsync(request.Id, request.OrgCode, request.Availability);
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

                    int id = await _matchRecordRepository.GetMatchApplicant(item.RequirementId, item.ResourceId);

                    dynamic obj = new ExpandoObject();
                    obj.IsApplied = id > 0;

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
                    obj.LocationType = requirement.LocationType;

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
                .Where(x =>
                    string.IsNullOrEmpty(request.SearchText) || (
                        (x.Title != null && x.Title.ToString().IndexOf(request.SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (x.Description != null && x.Description.ToString().IndexOf(request.SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    ))
                .Where(x =>
                    request.Status == null || request.Status.Count == 0 || request.Status.Contains((int)x.Status))
                .Where(x =>
                    request.LocationType == null || request.LocationType.Count == 0 || request.LocationType.Contains(x.LocationType))
                .ToList();

            dynamic result = new ExpandoObject();
            result.MatchingRecordCount = filteredList.Count;
            result.Records = filteredList;

            return new List<dynamic> { result };
        }


        public async Task<bool> UpsertApplicantStatusHistory(ApplicantStatusHistory model)
        {
            try
            {
                if (model == null || model.ApplicantId <= 0 || model.Status <= 0)
                    return false;

                model.ChangedOn = DateTime.UtcNow;
                var result = await _benchRepository.InsertApplicantStatusHistory(model);

                try
                {
                    var orgObject = await _resourcesRepository.GetApplicationWithVendorAndResourceByIdAsync(model.ApplicantId);
                    if (orgObject != null)
                    {
                        string status = Enum.GetName(typeof(RecruitmentStatus), model.Status);
                        try
                        {
                            var orgName=await _organizationRepository.GetOrganizationData(model.ChangedBy);
                            model.ChangedBy = orgName.OrgName;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        string message = $"Applicant with name {orgObject.ResourceName} status changed to {status} by {model.ChangedBy}";
                        string title = $"Applicant Status Changed: {orgObject.ResourceName}";   

                        await _notificationRepository.InsertNotificationAsync(
                            orgObject.VendorCode,
                            message,
                            (int)NotificationType.ResourceStatusChanged,
                            title
                        );
                        var notificationData = new
                        {
                            Message = message,
                            OrgCode = orgObject.VendorCode,
                            NotificationType = (int)NotificationType.ResourceStatusChanged,
                            CreatedOn = DateTime.UtcNow,
                            title
                        };

                        await _hubContext.Clients.Group(orgObject.VendorCode).SendAsync("ReceiveNotification", notificationData);
                    }
                }
                catch (Exception ex)
                {
                   
                }

                return result;
            }
            catch (Exception ex)
            {
                // Optionally log the outer exception too
                Console.WriteLine($"Error during UpsertApplicantStatusHistory: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ApplicantStatusHistoryResponse>> GetApplicantStatusHistory(int applicantId)
        {
            var history = await _benchRepository.GetStatusHistoryByApplicantId(applicantId);
            var result= new  List<ApplicantStatusHistoryResponse>();
            if (history != null && history.Any())
            {

                 result = history.Select(item => new ApplicantStatusHistoryResponse
                {
                    Status = item.Status,
                    StatusName = EnumHelper.GetEnumDescription<RecruitmentStatus>(item.Status),
                    ChangedBy = item.ChangedBy,
                    ChangedOn = item.ChangedOn,
                     Comment= item.Comment

                 }).ToList();
            }

            return result;
        }

    }
}


public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
       
        return connection.User?.FindFirst("OrgCode")?.Value;
    }
}

