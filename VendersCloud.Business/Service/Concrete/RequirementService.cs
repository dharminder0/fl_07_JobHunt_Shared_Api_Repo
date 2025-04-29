using Azure.Core;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using VendersCloud.Business.CommonMethods;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Concrete;

namespace VendersCloud.Business.Service.Concrete
{
    public class RequirementService : IRequirementService
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IBenchRepository _benchRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ISkillRequirementMappingRepository _skillRequirementMappingRepository;
        private readonly IMatchRecordRepository _matchRecordRepository;
        private readonly IOrgRelationshipsRepository _orgRelationshipsRepository;
        private readonly IRequirementVendorsRepository _requirementVendorsRepository;
        private readonly IOrgLocationRepository _organizationLocationRepository;
        private readonly IListValuesRepository _listValuesRepository;
        private readonly IPartnerVendorRelRepository _partnerVendorRelRepository;
        public RequirementService(IRequirementRepository requirementRepository, IClientsRepository clientsRepository, IResourcesRepository resourcesRepository,
            IBenchRepository benchRepository, IUsersRepository usersRepository, IOrganizationRepository organizationRepository,
            ISkillRepository skillRepository, ISkillRequirementMappingRepository skillRequirementMappingRepository, IMatchRecordRepository matchRecordRepository,
            IOrgRelationshipsRepository orgRelationshipsRepository, IRequirementVendorsRepository requirementVendorsRepository,
            IOrgLocationRepository organizationLocationRepository, IListValuesRepository listValuesRepository, IPartnerVendorRelRepository partnerVendorRelRepository)
        {
            _requirementRepository = requirementRepository;
            _clientsRepository = clientsRepository;
            _resourcesRepository = resourcesRepository;
            _benchRepository = benchRepository;
            _usersRepository = usersRepository;
            _organizationRepository = organizationRepository;
            _skillRepository = skillRepository;
            _skillRequirementMappingRepository = skillRequirementMappingRepository;
            _matchRecordRepository = matchRecordRepository;
            _orgRelationshipsRepository = orgRelationshipsRepository;
            _requirementVendorsRepository = requirementVendorsRepository;
            _organizationLocationRepository = organizationLocationRepository;
            _listValuesRepository = listValuesRepository;
            _partnerVendorRelRepository = partnerVendorRelRepository;
        }

        public async Task<ActionMessageResponse> RequirmentUpsertAsync(RequirementRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.OrgCode) || string.IsNullOrEmpty(request.UserId))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                var uniqueId = Guid.NewGuid().ToString().Substring(0, 12);
                var response = await _requirementRepository.RequirementUpsertAsync(request, uniqueId);
                if (response != null)
                {
                    var requirementdata = await _requirementRepository.GetRequirementListByIdAsync(response.UniqueId);
                    if (requirementdata != null)
                    {
                        int requirementId = requirementdata.FirstOrDefault().Id;
                        if (request.Skills != null)
                        {
                            var data = await _skillRepository.SkillUpsertAsync(request.Skills);
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    await _skillRequirementMappingRepository.UpsertSkillRequirementMappingAsync(item.Id, requirementId);
                                }
                            }
                        }
                        dynamic res = new ExpandoObject();
                        res.Id = response.Id;
                        res.UniqueId = response.UniqueId;
                        return new ActionMessageResponse() { Success = true, Message = "Requirement Submitted Successfully!! ", Content = res };
                    }
                }
                return new ActionMessageResponse() { Success = false, Message = "Requirement Not Submitted  ", Content = "" };

            }
            catch (Exception ex)
            {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse> DeleteRequirementAsync(int requirementId, string orgCode)
        {
            try
            {
                if ((requirementId <= 0) || (string.IsNullOrEmpty(orgCode)))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                bool response = await _requirementRepository.DeleteRequirementAsync(requirementId, orgCode);
                if (response)
                {
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Deleted Successfully!! ", Content = "" };
                }
                return new ActionMessageResponse() { Success = false, Message = "Requirement Not Deleted!!  ", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<List<RequirementResponse>> GetRequirementListAsync()
        {
            try
            {
                var res = new List<RequirementResponse>();
                var response = await _requirementRepository.GetRequirementListAsync();
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var requirementResponse = new RequirementResponse
                        {
                            Id = item.Id,
                            Title = item.Title,
                            OrgCode = item.OrgCode,
                            Description = item.Description,
                            Experience = item.Experience,
                            Budget = item.Budget,
                            Positions = item.Positions,
                            Duration = item.Duration,
                            LocationType = item.LocationType,
                            LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType),
                            Location = item.Location,
                            ClientCode = item.ClientCode,
                            Remarks = item.Remarks,
                            Visibility = item.Visibility,
                            VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility),
                            Hot = item.Hot,
                            Status = item.Status,
                            StatusName = Enum.GetName(typeof(RequirementsStatus), item.Status),
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.UpdatedBy,
                            IsDeleted = item.IsDeleted,
                            UniqueId = item.UniqueId
                        };

                        var orgData = await _clientsRepository.GetClientsByClientCodeAsync(item.ClientCode);
                        if (orgData != null)
                        {
                            requirementResponse.ClientName = orgData.ClientName;
                            requirementResponse.ClientFavicon = orgData.FaviconURL;
                        }

                        res.Add(requirementResponse);
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RequirementResponse> GetRequirementListByIdAsync(string requirementId)
        {
            try
            {
                if (string.IsNullOrEmpty(requirementId))
                {
                    return new RequirementResponse();
                }

                RequirementResponse res = new RequirementResponse();
                var skillMappingData = new List<SkillRequirementMapping>();
                List<int> Ids = new List<int>();
                var response = await _requirementRepository.GetRequirementListByIdAsync(requirementId);
               
                    if (response != null)
                    {
                        foreach (var item in response)
                        {
                            var orgData = await _clientsRepository.GetClientsByClientCodeAsync(item.ClientCode);
                            if (orgData != null)
                            {
                                res.ClientName = orgData.ClientName;
                                res.ClientFavicon = orgData.FaviconURL;
                            }
                        skillMappingData = await _skillRequirementMappingRepository.GetSkillRequirementMappingAsync(item.Id);
                        if (skillMappingData != null)
                        {
                            foreach (var skill in skillMappingData)
                            {
                                Ids.Add(skill.SkillId);

                            }
                            var skillName = await _skillRepository.GetAllSkillNamesAsync(Ids);
                            res.Id = item.Id;
                            res.Title = item.Title;
                            res.OrgCode = item.OrgCode;
                            res.Description = item.Description;
                            res.Experience = item.Experience;
                            res.Budget = item.Budget;
                            res.Positions = item.Positions;
                            res.Duration = item.Duration;
                            res.LocationType = item.LocationType;
                            res.LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType);
                            res.Location = item.Location;
                            res.ClientCode = item.ClientCode;
                            res.Remarks = item.Remarks;
                            res.Visibility = item.Visibility;
                            res.VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility);
                            res.Hot = item.Hot;
                            res.Status = item.Status;
                            res.StatusName = Enum.GetName(typeof(RequirementsStatus), item.Status);
                            res.CreatedOn = item.CreatedOn;
                            res.UpdatedOn = item.UpdatedOn;
                            res.CreatedBy = item.CreatedBy;
                            res.UpdatedBy = item.UpdatedBy;
                            res.IsDeleted = item.IsDeleted;
                            res.UniqueId = item.UniqueId;
                            res.Skills = skillName;
                        }
                    }
                }
                return res;
               
            }
            catch (Exception ex)
            {
                // Add more context to the exception
                throw new Exception("An error occurred while fetching the requirement list.", ex);
            }
        }


        public async Task<ActionMessageResponse> UpdateStatusByIdAsync(int requirementId, int status)
        {
            try
            {
                if (requirementId <= 0 || status <= 0)
                {
                    return new ActionMessageResponse { Success = false, Message = "Enter Valid Input!!", Content = "" };
                }
                var res = await _requirementRepository.UpdateStatusByIdAsync(requirementId, status);
                if (res)
                    return new ActionMessageResponse { Success = true, Message = "Status Updated Successfully!!", Content = "" };
                return new ActionMessageResponse { Success = false, Message = "Status Not Updated", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<List<RequirementResponse>> GetRequirementByOrgCodeAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    return new List<RequirementResponse>();
                }

                List<int> Ids = new List<int>();
                var skillMappingData = new List<SkillRequirementMapping>();
                var res = new List<RequirementResponse>();
                var response = await _requirementRepository.GetRequirementByOrgCodeAsync(orgCode);

                if (response != null)
                {
                    foreach (var item in response)
                    {
                        skillMappingData = await _skillRequirementMappingRepository.GetSkillRequirementMappingAsync(item.Id);
                        if (skillMappingData != null)
                        {
                            foreach (var skill in skillMappingData)
                            {
                                Ids.Add(skill.SkillId);

                            }
                            var skillName = await _skillRepository.GetAllSkillNamesAsync(Ids);

                            var requirementResponse = new RequirementResponse
                            {
                                Id = item.Id,
                                Title = item.Title,
                                OrgCode = item.OrgCode,
                                Description = item.Description,
                                Experience = item.Experience,
                                Budget = item.Budget,
                                Positions = item.Positions,
                                Duration = item.Duration,
                                LocationType = item.LocationType,
                                LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType),
                                Location = item.Location,
                                ClientCode = item.ClientCode,
                                Remarks = item.Remarks,
                                Visibility = item.Visibility,
                                VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility),
                                Hot = item.Hot,
                                Status = item.Status,
                                StatusName = Enum.GetName(typeof(RequirementsStatus), item.Status),
                                CreatedOn = item.CreatedOn,
                                UpdatedOn = item.UpdatedOn,
                                CreatedBy = item.CreatedBy,
                                UpdatedBy = item.UpdatedBy,
                                IsDeleted = item.IsDeleted,
                                UniqueId = item.UniqueId,
                                Skills = skillName,
                            };

                            var orgData = await _clientsRepository.GetClientsByClientCodeAsync(item.ClientCode);
                            if (orgData != null)
                            {
                                requirementResponse.ClientName = orgData.ClientName;
                                requirementResponse.ClientFavicon = orgData.FaviconURL;
                            }

                            res.Add(requirementResponse);
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginationDto<RequirementResponse>> SearchRequirementAsync(SearchRequirementRequest request)
        {
            try
            {
                int totalRecords = 0;
                List<string> skillData = new List<string>();
                List<Requirement> paginatedRequirements;
                List<Requirement> filteredEmplanelRequirement = new List<Requirement>();
                List<int> matchingCandidate;
                List<int> RequirementVendorsId;
                if (string.IsNullOrEmpty(request.OrgCode) || string.IsNullOrEmpty(request.UserId))
                {
                    throw new Exception("OrgCode is Mandatory!!");
                }
                int place, Applicants = 0;
                var requirements = await _requirementRepository.GetRequirementsListAsync(request);
                var emplanedRequirements = await _requirementRepository.GetRequirementListAsync();
                var orgRelationshipdata = await _partnerVendorRelRepository.GetBenchResponseListByIdAsync(request.OrgCode);
                RequirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);
                var sharedrequirement = await _requirementRepository.GetRequirementByIdAsync(RequirementVendorsId);
                foreach (var rel in orgRelationshipdata)
                {
                    if (rel.PartnerCode == request.OrgCode)
                    {
                        var reqdata = emplanedRequirements.Where(x => x.OrgCode == rel.VendorCode && x.Visibility == 2);
                        filteredEmplanelRequirement.AddRange(reqdata);
                    }
                    else
                    {
                        var reqdata = emplanedRequirements.Where(x => (x.OrgCode == rel.PartnerCode && x.Visibility == 2));
                        filteredEmplanelRequirement.AddRange(reqdata);
                    }
                }

                if (request.RoleType.Contains("1"))
                {
                    var visibleRequirements = await _requirementRepository.GetRequirementsListByVisibilityAsync(request);
                    var allRequirements = requirements.Concat(visibleRequirements).Concat(filteredEmplanelRequirement).Concat(sharedrequirement).Distinct().ToList();
                    totalRecords = allRequirements.Count;
                    paginatedRequirements = allRequirements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                }
                else
                {
                    var allRequirements = requirements.Concat(filteredEmplanelRequirement).Concat(sharedrequirement).Distinct().ToList();
                    totalRecords = allRequirements.Count;
                    paginatedRequirements = allRequirements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                }


                TotalApplicantsRequest ApplicantSearch = new TotalApplicantsRequest();

                var requirementsResponseList = new List<RequirementResponse>();
                foreach (var r in paginatedRequirements)
                {
                    ApplicantSearch.RequirementUniqueId = r.UniqueId;
                    ApplicantSearch.Status = 8;
                    Applicants = await _resourcesRepository.GetTotalApplicationsPerRequirementIdAsync(r.Id);
                    matchingCandidate = await _matchRecordRepository.GetMatchingCountByRequirementId(r.Id);
                    var data = await _benchRepository.GetBenchResponseListByIdAsync(matchingCandidate);
                    var resourcesList = data.Where(x => x.OrgCode == request.OrgCode);
                    var count = resourcesList.Count();
                    place = await GetTotalApplicantsAsync(ApplicantSearch);
                    var skillMappingData = await _skillRequirementMappingRepository.GetSkillRequirementMappingAsync(r.Id);
                    List<int> SkillId = new List<int>();
                    if (skillMappingData != null && skillMappingData.Count > 0)
                    {
                        foreach (var item in skillMappingData)
                        {
                            SkillId.Add(item.SkillId);
                        }
                    }

                    var requirementResponse = new RequirementResponse
                    {
                        Id = r.Id,
                        Title = r.Title,
                        OrgCode = r.OrgCode,
                        Description = r.Description,
                        Experience = r.Experience,
                        Budget = r.Budget,
                        Positions = r.Positions,
                        Duration = r.Duration,
                        LocationType = r.LocationType,
                        LocationTypeName = System.Enum.GetName(typeof(LocationType), r.LocationType),
                        Location = r.Location,
                        ClientCode = r.ClientCode,
                        Remarks = r.Remarks,
                        Visibility = r.Visibility,
                        VisibilityName = System.Enum.GetName(typeof(Visibility), r.Visibility),
                        Hot = r.Hot,
                        Status = r.Status,
                        Placed = place,
                        Applicants = Applicants,
                        StatusName = System.Enum.GetName(typeof(RequirementsStatus), r.Status),
                        CreatedOn = r.CreatedOn,
                        UpdatedOn = r.UpdatedOn,
                        CreatedBy = r.CreatedBy,
                        UpdatedBy = r.UpdatedBy,
                        IsDeleted = r.IsDeleted,
                        MatchingCandidates = count,
                        UniqueId = r.UniqueId
                    };
                    if (SkillId.Count > 0)
                    {
                        skillData = await _skillRepository.GetAllSkillNamesAsync(SkillId);
                        requirementResponse.Skills = skillData;

                    }
                    var orgData = await _clientsRepository.GetClientsByClientCodeAsync(r.ClientCode);
                    if (orgData != null)
                    {
                        requirementResponse.ClientName = orgData.ClientName;
                        requirementResponse.ClientFavicon = orgData.FaviconURL;
                    }
                    else
                    {
                        var clientData = await _organizationRepository.GetOrganizationData(r.ClientCode);
                        if (clientData != null)
                        {
                            requirementResponse.ClientName = clientData.OrgName;
                            requirementResponse.ClientFavicon = clientData.Logo;
                        }
                    }

                    requirementsResponseList.Add(requirementResponse);
                }

                return new PaginationDto<RequirementResponse>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    List = requirementsResponseList
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalApplicantsAsync(TotalApplicantsRequest request)
        {
            try
            {
                int requirementId = 0;
                if (request == null || string.IsNullOrEmpty(request.RequirementUniqueId))
                {
                    throw new Exception("Enter Valid Inputs");
                }
                var requiementData = await _requirementRepository.GetRequirementListByIdAsync(request.RequirementUniqueId);
                if (requiementData != null)
                {
                    foreach (var item in requiementData)
                    {
                        requirementId = item.Id;
                    }
                }
                var applicantData = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(requirementId, request.Status);
                var totalApplicants = applicantData.Count;
                return totalApplicants;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginationDto<ApplicationListResponse>> GetApplicantsListByRequirementIdAsync(GetApplicantsByRequirementRequest request)
        {
            if (string.IsNullOrEmpty(request.RequirementUniqueId))
            {
                throw new ArgumentException("Requirement ID cannot be null or empty");
            }

            var listResponse = new List<ApplicationListResponse>();

            try
            {
                var requirementData = await _requirementRepository.GetRequirementListByIdAsync(request.RequirementUniqueId);
                if (requirementData == null)
                {
                    return null;
                }

                foreach (var requirementItem in requirementData)
                {
                    var applicationData = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(requirementItem.Id);
                    if (applicationData == null) continue;

                    foreach (var applicationItem in applicationData)
                    {
                        var benchData = await _benchRepository.GetBenchResponseByIdAsync(applicationItem.ResourceId);
                        if (benchData == null || !benchData.Any()) continue;

                        var benchMember = benchData.First();

                        // Filtering by search text if provided
                        if (!string.IsNullOrEmpty(request.SearchText) &&
                            !benchMember.FirstName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) &&
                            !benchMember.LastName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // Filtering by status if provided
                        if (request.Status != null && request.Status.Any() && !request.Status.Contains(applicationItem.Status))
                        {
                            continue;
                        }

                        var applicationResponse = new ApplicationListResponse
                        {
                            Title = requirementItem.Title,
                            RequirementId = requirementItem.Id,
                            Status = applicationItem.Status,
                            StatusName = System.Enum.GetName(typeof(ApplyStatus), applicationItem.Status),
                            ApplicationDate = applicationItem.CreatedOn,
                            FirstName = benchMember.FirstName,
                            LastName = benchMember.LastName,
                            VendorOrgCode = benchMember.OrgCode
                        };

                        var orgdata = await _organizationRepository.GetOrganizationData(benchMember.OrgCode);
                        if (orgdata != null)
                        {
                            applicationResponse.vendorLogo = orgdata.Logo;
                            applicationResponse.VendorOrgName = orgdata.OrgName;
                        }

                        listResponse.Add(applicationResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            var totalRecords = listResponse.Count;
            return new PaginationDto<ApplicationListResponse>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = listResponse
            };
        }


        public async Task<PaginationDto<CompanyRequirementResponse>> GetRequirementListByOrgCode(CompanyRequirementSearchRequest request)
        {
            try
            {
                // Validate input
                if (request == null || string.IsNullOrEmpty(request.OrgCode))
                {
                    throw new ArgumentNullException("Enter Valid Input!!");
                }

                List<CompanyRequirementResponse> listResponse = new List<CompanyRequirementResponse>();

                // Get organization and requirement data
                var orgData = await _organizationRepository.GetOrganizationData(request.OrgCode);
                var requirementData = await _requirementRepository.GetRequirementByOrgCodeAsync(request.OrgCode);

                // Filter requirements by client criteria
                if (requirementData != null && requirementData.Any())
                {
                    var filteredRequirements = requirementData
                        .Where(item =>
                            (request.Client == null || !request.Client.Any() || request.Client.Contains(item.ClientCode))
                        )
                        .ToList();

                    foreach (var item in filteredRequirements)
                    {
                        // Get application data
                        var allApplications = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(item.Id);

                        if (allApplications == null || !allApplications.Any())
                        {
                            continue; // Skip if there are no applications for this requirement
                        }

                        // Filter applications by Status
                        var filteredApplications = allApplications
                            .Where(app => request.Status == null || !request.Status.Any() || request.Status.Contains(app.Status))
                            .ToList();

                        foreach (var app in filteredApplications)
                        {
                            // Create a response object for each application
                            var requirementResponse = new CompanyRequirementResponse
                            {
                                RequirementUniqueId = item.UniqueId,
                                RequirementId = item.Id,
                                Role = item.Title,
                                ClientCode = item.ClientCode,
                                Position = item.Positions,
                                ApplicationDate = item.CreatedOn,
                                OrgName = orgData.OrgName,
                                OrgLogo = orgData.Logo,
                                Status = app.Status,
                                StatusName = CommonFunctions.GetEnumDescription((ApplyStatus)app.Status)
                            };

                            var vendorDetails = await _usersRepository.GetUserByIdAsync(app.CreatedBy);
                            var vendorOrgData = await _organizationRepository.GetOrganizationData(vendorDetails.OrgCode);
                            requirementResponse.Comment = app.Comment;
                            requirementResponse.VendorOrgName = vendorOrgData.OrgName;
                            requirementResponse.VendorLogo = vendorOrgData.Logo;
                            requirementResponse.VendorOrgCode = vendorOrgData.OrgCode;
                            requirementResponse.ResourceId = app.ResourceId;
                            var benchData = await _benchRepository.GetBenchResponseByIdAsync(app.ResourceId);
                            var candidateDetails = benchData?.FirstOrDefault();

                            if (candidateDetails != null)
                            {
                                requirementResponse.FirstName = candidateDetails.FirstName;
                                requirementResponse.LastName = candidateDetails.LastName;
                                requirementResponse.CV = candidateDetails.CV;
                            }

                            requirementResponse.Applicants = await _resourcesRepository.GetTotalApplicationsPerRequirementIdAsync(requirementResponse.RequirementId);

                            var clientData = await _clientsRepository.GetClientsByClientCodeAsync(requirementResponse.ClientCode);
                            if (clientData != null)
                            {
                                requirementResponse.ClientName = clientData.ClientName;
                                requirementResponse.ClientLogo = clientData.LogoURL;
                            }

                            // Add response to the list
                            listResponse.Add(requirementResponse);
                        }
                    }
                }

                // Apply SearchText filter
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    listResponse = listResponse
                        .Where(req =>
                            (!string.IsNullOrEmpty(req.FirstName) && req.FirstName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(req.LastName) && req.LastName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }

                // Pagination logic
                int totalRecords = listResponse.Count;
                var paginatedRequirements = listResponse
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PaginationDto<CompanyRequirementResponse>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    List = paginatedRequirements
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CompanyDashboardCountResponse> GetCountsAsync(string orgCode)
        {
            try
            {
                return await _requirementRepository.GetCountsAsync(orgCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId)
        {
            try
            {
                return await _requirementRepository.GetVendorsCountsAsync(orgCode, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CompanyGraphResponse>> GetDayWeekCountsAsync(CompanyGraphRequest request)
        {
            try
            {
                var data = await _requirementRepository.GetOrgTotalPlacementAndRequirementIdAsync(request);
                var finalResult = new List<CompanyGraphResponse>();

                foreach (var item in data)
                {
                    var requirementIds = item.RequirementIds != null ? ((string)item.RequirementIds).Split(',').Select(int.Parse).ToList() : new List<int>();

                    int totalPlacements = await _resourcesRepository.GetTotalPlacementsAsync(requirementIds);

                    finalResult.Add(new CompanyGraphResponse
                    {
                        OrgCode = item.OrgCode,
                        WeekDay = item.WeekDay,
                        TotalPositions = item.TotalPositions,
                        TotalPlacements = totalPlacements
                    });
                }

                return finalResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetDayWeekCountsAsync: {ex.Message}", ex);
            }
        }

        public async Task<dynamic> GetRequirementCountsAsync(CompanyGraphRequest request)
        {
            try
            {
                var data = await _requirementRepository.GetRequirementCountAsync(request);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<dynamic> GetVendorRequirementCountsAsync(VendorGraphRequest request)
        {
            try
            {
                var data = await _requirementRepository.GetVendorRequirementCountAsync(request);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<VendorGraphResponse>> GetVendorDayWeekCountsAsync(VendorGraphRequest request)
        {
            try
            {
                var data = await _requirementRepository.GetVendorTotalPlacementAndRequirementIdAsync(request);
                var finalResult = new List<VendorGraphResponse>();

                foreach (var item in data)
                {
                    var requirementIds = item.RequirementIds != null ? ((string)item.RequirementIds).Split(',').Select(int.Parse).ToList() : new List<int>();

                    int totalPlacements = await _resourcesRepository.GetTotalPlacementsAsync(requirementIds);

                    finalResult.Add(new VendorGraphResponse
                    {
                        OrgCode = item.OrgCode,
                        WeekDay = item.WeekDay,
                        TotalPositions = item.TotalPositions,
                        TotalPlacements = totalPlacements
                    });
                }

                return finalResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetDayWeekCountsAsync: {ex.Message}", ex);
            }
        }

        public async Task<ActionMessageResponse> HotRequirementUpsertAsync(HotRequirementRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.RequirementUniqueId) || request.Hot < 0)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                var response = await _requirementRepository.UpdateHotByIdAsync(request.RequirementUniqueId, request.Hot);
                if (response)
                {
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Updated Successfully!! ", Content = "" };
                }
                return new ActionMessageResponse() { Success = false, Message = "Requirement Not Updated  ", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<PaginationDto<dynamic>> GetHotRequirementAsync(GetHotRequirmentRequest request)
        {
            try
            {
                List<Requirement> requirements = await _requirementRepository.GetRequirementByOrgCodeAsync(request.OrgCode);
                var hotRequirements = requirements.Where(r => r.Hot).ToList();

                // Safely filter out null/empty ClientCodes
                var clientCodes = hotRequirements
                    .Select(r => r.ClientCode)
                    .Where(code => !string.IsNullOrWhiteSpace(code))
                    .Distinct()
                    .ToList();

                Dictionary<string, dynamic> clientData = new();

                foreach (var clientCode in clientCodes)
                {
                    var client = await _clientsRepository.GetClientsByClientCodeAsync(clientCode);
                    if (client != null && !string.IsNullOrWhiteSpace(client.ClientCode))
                    {
                        dynamic obj = new ExpandoObject();
                        obj.ClientName = client.ClientName;
                        obj.LogoURL = client.LogoURL;
                        obj.ClientCode = client.ClientCode;
                        clientData[client.ClientCode] = obj;
                    }
                    else
                    {
                        var orgData = await _organizationRepository.GetOrganizationData(clientCode);
                        if (orgData != null && !string.IsNullOrWhiteSpace(orgData.OrgCode))
                        {
                            dynamic obj = new ExpandoObject();
                            obj.ClientName = orgData.OrgName;
                            obj.LogoURL = orgData.Logo;
                            obj.ClientCode = orgData.OrgCode;
                            clientData[orgData.OrgCode] = obj;
                        }
                    }
                }

                var result = new List<dynamic>();
                foreach (var req in hotRequirements)
                {
                    if (!string.IsNullOrWhiteSpace(req.ClientCode) && clientData.TryGetValue(req.ClientCode, out dynamic client))
                    {
                        dynamic obj = new ExpandoObject();
                        obj.ClientName = client.ClientName;
                        obj.ClientLogo = client.LogoURL;
                        obj.ClientCode = client.ClientCode;
                        obj.Title = req.Title;
                        obj.Positions = req.Positions;
                        obj.CreatedOn = req.CreatedOn;
                        obj.Visibility = req.Visibility;
                        obj.VisibilityName = Enum.GetName(typeof(Visibility), req.Visibility);
                        obj.LocationType = req.LocationType;
                        obj.LocationTypeName = Enum.GetName(typeof(LocationType), req.LocationType);
                        obj.RequirementUniqueId = req.UniqueId;
                        obj.Hot = req.Hot;
                        result.Add(obj);
                    }
                    else
                    {
                        // fallback if no client info
                        dynamic obj = new ExpandoObject();
                        obj.ClientName = "";
                        obj.ClientLogo = "";
                        obj.ClientCode = "";
                        obj.Title = req.Title;
                        obj.Positions = req.Positions;
                        obj.CreatedOn = req.CreatedOn;
                        obj.Visibility = req.Visibility;
                        obj.VisibilityName = Enum.GetName(typeof(Visibility), req.Visibility);
                        obj.LocationType = req.LocationType;
                        obj.LocationTypeName = Enum.GetName(typeof(LocationType), req.LocationType);
                        obj.RequirementUniqueId = req.UniqueId;
                        obj.Hot = req.Hot;
                        result.Add(obj);
                    }
                }

                int totalRecords = result.Count;
                var pagedResult = result
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PaginationDto<dynamic>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    List = pagedResult
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching hot requirements: " + ex.Message);
            }
        }

        public async Task<List<dynamic>> GetRequirementMatchResultAsync(RequirementMatchRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OrgCode) || request.RequirementId <= 0)
                {
                    throw new ArgumentException("Enter valid inputs");
                }

                List<dynamic> result = new List<dynamic>();
                List<int> matchingCandidate = await _matchRecordRepository.GetMatchingCountByRequirementId(request.RequirementId);
                var benchData = await _benchRepository.GetBenchResponseListByIdAsync(matchingCandidate);

                foreach (var item in benchData)
                {
                    dynamic obj = new ExpandoObject();

                    var matchScoreResult = await _matchRecordRepository.GetMatchScoreAsync(request.RequirementId, item.Id);
                    obj.MatchScore = matchScoreResult.MatchScore;

                    obj.FirstName = item.FirstName;
                    obj.LastName = item.LastName;
                    obj.Email = item.Email;
                    obj.BenchId = item.Id;
                    obj.Title = item.Title;
                    obj.ResourceOrgCode = item.OrgCode;
                    obj.Cv = await GetCvByIdAsync(item.Id);
                    obj.CreatedOn = item.CreatedOn;
                    obj.CreatedBy = item.CreatedBy;
                    obj.IsDeleted = item.IsDeleted;
                    obj.UpdatedBy = item.UpdatedBy;
                    obj.UpdatedOn = item.UpdatedOn;
                    obj.Availability = item.Availability;
                    obj.AvailabilityName = CommonFunctions.GetEnumDescription((BenchAvailability)item.Availability);

                    result.Add(obj);
                }

                var filteredList = result
                    .Where(x => x.ResourceOrgCode == request.OrgCode)
                    .ToList();

                dynamic resultList = new ExpandoObject();
                resultList.MatchingRecordCount = filteredList.Count;
                resultList.Records = filteredList;

                return new List<dynamic> { resultList };
            }
            catch (Exception)
            {
                throw;
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
        public async Task<string> GetAvtarByIdAsync(int benchId)
        {
            try
            {
                var link = await _benchRepository.GetAvtarByIdAsync(benchId);
                return link.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> GetMatchingVendorsAsync(MatchingVendorRequest request)
        {
            try
            {
                List<dynamic> result = new List<dynamic>();

                if (string.IsNullOrEmpty(request.OrgCode) || request.RequirementId <= 0)
                {
                    throw new ArgumentException("Enter valid inputs");
                }

                var matchingdata = await _matchRecordRepository.GetMatchingResultByRequirementId(request.RequirementId);

                var empaneledOrgs = await _orgRelationshipsRepository.GetBenchResponseListByIdAsync(request.OrgCode);
                var empOrgCodes = empaneledOrgs
                    .Where(x => x.OrgCode == request.OrgCode || x.RelatedOrgCode == request.OrgCode)
                    .Select(x => x.OrgCode == request.OrgCode ? x.RelatedOrgCode : x.OrgCode)
                    .Distinct()
                    .ToList();

                foreach (var item in matchingdata)
                {
                    dynamic obj = new ExpandoObject();
                    obj.City = null;
                    obj.State = null;
                    obj.StateName = null;
                    obj.MatchingScore = item.MatchScore;
                    obj.MatchingResourceId = item.ResourceId;
                    var resourcedata = await _benchRepository.GetBenchResponseByIdAsync(item.ResourceId);

                    if (resourcedata != null && resourcedata.Count > 0)
                    {
                        var rdata = resourcedata[0];
                        obj.MatchingOrgCode = rdata.OrgCode;

                        var orgdata = await _organizationRepository.GetOrganizationData(rdata.OrgCode);
                        var orgLocationData = await _organizationLocationRepository.GetOrgLocation(rdata.OrgCode);
                        foreach (var dataLocation in orgLocationData)
                        {
                            var data = await _listValuesRepository.GetListValuesAsync();
                            var selectedValues = data.Where(x => x.Id == dataLocation.State).Select(x => x.Value).FirstOrDefault();
                            obj.City = dataLocation.City;
                            obj.State = dataLocation.State;
                            obj.StateName = selectedValues;
                          
                        }
                        obj.OrgName = orgdata.OrgName;
                        obj.OrgLogo = orgdata.Logo;
                        result.Add(obj);
                    }
                }


                var grouped = result
                    .GroupBy(r => r.MatchingOrgCode)
                    .Select(g => new
                    {
                        MatchingOrgCode = g.Key,
                        OrgName = g.First().OrgName,
                        OrgLogo = g.First().OrgLogo,
                        City=g.First().City,
                        State=g.First().State,
                        StateName=g.First().StateName,
                        AverageMatchingScore = g.Average(x => (double)x.MatchingScore),
                        NumberOfCandidates = g
                        .Select(x => x.MatchingResourceId)
                        .Distinct()
                        .Count()
                    });

                var empaneledVendors = grouped
                    .Where(g => empOrgCodes.Contains(g.MatchingOrgCode))
                    .ToList();

                var publicVendors = grouped
                    .Where(g => !empOrgCodes.Contains(g.MatchingOrgCode) && g.MatchingOrgCode != request.OrgCode)
                    .ToList();

                return new
                {
                    EmpaneledVendor = empaneledVendors,
                    PublicVendor = publicVendors
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
