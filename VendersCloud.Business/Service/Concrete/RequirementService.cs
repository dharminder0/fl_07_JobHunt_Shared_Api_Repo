using Azure.Core;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using VendersCloud.Business.CommonMethods;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Common.Extensions;
using VendersCloud.Data.Repositories.Concrete;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VendersCloud.Common.Extensions.StringExtensions;
using static VendersCloud.Data.Repositories.Concrete.ResourcesRepository;

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
                        var partnerdata = await _organizationRepository.GetOrganizationData(item.OrgCode);
                        if(partnerdata != null)
                        {
                            res.PartnerCode = partnerdata.OrgCode;
                            res.PartnerName = partnerdata.OrgName;
                            res.PartnerFavicon = partnerdata.Logo;
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
        public async Task<PaginationDto<RequirementResponse>> SearchRequirementAsyncV2(SearchRequirementRequest request)
        {
            if (string.IsNullOrEmpty(request.OrgCode) || string.IsNullOrEmpty(request.UserId))
                throw new Exception("OrgCode is Mandatory!!");

            int totalRecords;
            var skillData = new List<string>();
            var requirementsResponseList = new List<RequirementResponse>();

            // Parallel DB calls
            var requirementsTask = _requirementRepository.GetRequirementsListAsync(request);
            var allReqTask = _requirementRepository.GetRequirementListAsync();
            var orgRelationsTask = _partnerVendorRelRepository.GetBenchResponseListByIdAsync(request.OrgCode);
            var sharedIdsTask = _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);

            await Task.WhenAll(requirementsTask, allReqTask, orgRelationsTask, sharedIdsTask);

            var requirements = requirementsTask.Result;
            var allReq = allReqTask.Result;
            var orgRelations = orgRelationsTask.Result;
            var sharedRequirementIds = sharedIdsTask.Result;

            var sharedRequirements = (await _requirementRepository.GetRequirementByIdAsync(sharedRequirementIds)).ToList();

            // Filter emplaned requirements
            var filteredEmplanelRequirement = orgRelations.SelectMany(rel =>
                allReq.Where(r =>
                    r.Visibility == 2 &&
                    ((rel.PartnerCode == request.OrgCode && r.OrgCode == rel.VendorCode) ||
                     (rel.PartnerCode != request.OrgCode && r.OrgCode == rel.PartnerCode)))
            ).ToList();

            List<Requirement> finalRequirements;
            if (request.RoleType.Contains("1"))
            {
                var visibleReqs = await _requirementRepository.GetRequirementsListByVisibilityAsync(request);
                finalRequirements = requirements.Concat(visibleReqs).Concat(sharedRequirements)
                                                .DistinctBy(r => r.Id).ToList();
            }
            else
            {
                finalRequirements = await _requirementRepository.GetRequirementByOrgCodeAsync(request.OrgCode);
            }

            totalRecords = finalRequirements.Count;
            var paginatedRequirements = finalRequirements
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            foreach (var r in paginatedRequirements)
            {
                var skillMapTask = _skillRequirementMappingRepository.GetSkillRequirementMappingAsync(r.Id);
                var applicantsTask = _resourcesRepository.GetTotalApplicationsPerRequirementIdAsync(r.Id);
                var matchTask = _matchRecordRepository.GetMatchingCountByRequirementId(r.Id);
                var placedTask = GetTotalApplicantsAsync(new TotalApplicantsRequest { RequirementUniqueId = r.UniqueId, Status = 8 });

                await Task.WhenAll(skillMapTask, applicantsTask, matchTask, placedTask);

                var skillMapping = skillMapTask.Result;
                var applicants = applicantsTask.Result;
                var matchCountIds = matchTask.Result;
                var placed = placedTask.Result;

                var benchData = await _benchRepository.GetBenchResponseListByIdAsync(matchCountIds);
                var matchingCount = benchData.Count(x => x.OrgCode == request.OrgCode);

                if (skillMapping?.Count > 0)
                {
                    var skillIds = skillMapping.Select(s => s.SkillId).ToList();
                    skillData = await _skillRepository.GetAllSkillNamesAsync(skillIds);
                }

                var clientsData = await _clientsRepository.GetClientsByClientCodeAsync(r.ClientCode);
                var organizationData = clientsData == null
                    ? await _organizationRepository.GetOrganizationData(r.ClientCode)
                    : null;

                var clientName = clientsData?.ClientName ?? organizationData?.OrgName;
                var clientLogo = clientsData?.FaviconURL ?? organizationData?.Logo;

             



                requirementsResponseList.Add(new RequirementResponse
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
                    LocationTypeName = Enum.GetName(typeof(LocationType), r.LocationType),
                    Location = r.Location,
                    ClientCode = r.ClientCode,
                    Remarks = r.Remarks,
                    Visibility = r.Visibility,
                    VisibilityName = Enum.GetName(typeof(Visibility), r.Visibility),
                    Hot = r.Hot,
                    Status = r.Status,
                    Placed = placed,
                    Applicants = applicants,
                    StatusName = Enum.GetName(typeof(RequirementsStatus), r.Status),
                    CreatedOn = r.CreatedOn,
                    UpdatedOn = r.UpdatedOn,
                    CreatedBy = r.CreatedBy,
                    UpdatedBy = r.UpdatedBy,
                    IsDeleted = r.IsDeleted,
                    MatchingCandidates = matchingCount,
                    UniqueId = r.UniqueId,
                    Skills = skillData,
                    ClientName = clientsData?.OrgCode ?? clientsData?.ClientName,
                    ClientFavicon = clientsData?.LogoURL ?? clientsData?.FaviconURL
                });
            }

            return new PaginationDto<RequirementResponse>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = requirementsResponseList
            };
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

                var emplanedRequirements = await _requirementRepository.GetRequirementListAsync();
                var orgRelationshipdata = await _partnerVendorRelRepository.GetBenchResponseListByIdAsync(request.OrgCode);
                RequirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);
                var sharedrequirement = await _requirementRepository.GetRequirementByIdAsync(RequirementVendorsId);
                var publicReq = await _requirementRepository.GetPublicRequirementAsync(orgRelationshipdata.Select(v => v.PartnerCode).ToList(), 3);
                
                    //var reqNoApplicants = _requirementRepository.GetRequirementsWithNoApplicantsAsync();
                
              
                foreach (var rel in orgRelationshipdata)
                {
                    if (rel.PartnerCode == request.OrgCode)
                    {
                        var reqdata = emplanedRequirements.Where(x => x.OrgCode == rel.VendorCode && x.Visibility == 2);
                        filteredEmplanelRequirement.AddRange(reqdata);
                    }
                    else
                    {
                        var sharedIds = sharedrequirement?.Select(s => s.Id).ToHashSet();

                        var reqdata = emplanedRequirements
                            .Where(x =>
                                x.OrgCode == rel.PartnerCode &&
                                x.Visibility == 2 &&
                                sharedIds != null &&
                                sharedIds.Contains(x.Id)
                            )
                            .ToList();


                        filteredEmplanelRequirement.AddRange(reqdata);
                    }
                }



                if (request.RoleType.Contains("1"))
                {
                    var allRequirements =
                        sharedrequirement
                        .Concat(publicReq)
                        .ToList();


                    if (request.LocationType != null && request.LocationType.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.LocationType.Contains(r.LocationType))
                            .ToList();
                    }
                    if (request.IsHotEnable)
                    {
                        allRequirements = allRequirements
                            .Where(r => r.Hot == request.IsHotEnable)
                            .ToList();
                    }

                    if (request.Status != null && request.Status.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.Status.Contains(r.Status))
                            .ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(request.SearchText))
                    {
                        allRequirements = allRequirements
                            .Where(r => !string.IsNullOrEmpty(r.Title) &&
                                        r.Title.IndexOf(request.SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                    }
                    if (request.ClientCode != null && request.ClientCode.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.ClientCode.Contains(r.ClientCode))
                            .ToList();
                    }
                    if (request.ApplicantsExist)
                    {
                        var reqNoApplicants = await _requirementRepository.GetRequirementsWithNoApplicantsAsync();
                        var noApplicantIds = reqNoApplicants.Select(r => r.Id).ToList();
                        allRequirements = allRequirements.Where(r => noApplicantIds.Contains(r.Id)).ToList();
                    }


                    totalRecords = allRequirements.Count;

                    paginatedRequirements = allRequirements
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();
                }

                else
                {
                    var allRequirements = await _requirementRepository.GetRequirementByOrgCodeAsync(request.OrgCode);

                    if (request.LocationType != null && request.LocationType.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.LocationType.Contains(r.LocationType))
                            .ToList();
                    }

                    if (request.Status != null && request.Status.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.Status.Contains(r.Status))
                            .ToList();
                    }
                    if (request.IsHotEnable)
                    {
                        allRequirements = allRequirements
                            .Where(r => r.Hot == request.IsHotEnable)
                            .ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(request.SearchText))
                    {
                        allRequirements = allRequirements
                            .Where(r => !string.IsNullOrEmpty(r.Title) &&
                                        r.Title.IndexOf(request.SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                    }
                    if (request.ClientCode != null && request.ClientCode.Any())
                    {
                        allRequirements = allRequirements
                            .Where(r => request.ClientCode.Contains(r.ClientCode))
                            .ToList();
                    }
                    if (request.ApplicantsExist)
                    {
                        var reqNoApplicants = await _requirementRepository.GetRequirementsWithNoApplicantsAsync();
                        var noApplicantIds = reqNoApplicants.Select(r => r.Id).ToList();
                        allRequirements = allRequirements.Where(r => noApplicantIds.Contains(r.Id)).ToList();
                    }
                    //var allRequirements = requirements.Concat(filteredEmplanelRequirement).Concat(sharedrequirement).Distinct().ToList();
                    totalRecords = allRequirements.Count;
                    paginatedRequirements = allRequirements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                }


                TotalApplicantsRequest ApplicantSearch = new TotalApplicantsRequest();

                var requirementsResponseList = new List<RequirementResponse>();
                foreach (var r in paginatedRequirements)
                {
                    int role = 0;
                    if (request.RoleType.Contains("1"))
                    {
                        role = 1;
                        
                    }
                    ApplicantSearch.RequirementUniqueId = r.UniqueId;
                    ApplicantSearch.Status = 8;
                    ApplicantSearch.VendorCode = request.OrgCode;
                    ApplicantSearch.Role = role;
                    Applicants = await _resourcesRepository.GetTotalApplicationsPerRequirementIdAsyncV2(r.Id,request.OrgCode,role);
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

                    var patnerData = await _clientsRepository.GetClientsByClientCodeAsync(r.OrgCode);
                    if (patnerData != null)
                    {
                        requirementResponse.PartnerName = patnerData.ClientName;
                        requirementResponse.PartnerFavicon = patnerData.FaviconURL;
                        requirementResponse.PartnerCode = r.OrgCode;
                    }
                    else
                    {
                        var patnerData1 = await _organizationRepository.GetOrganizationData(r.OrgCode);
                        if (patnerData1 != null)
                        {
                            requirementResponse.PartnerName = patnerData1.OrgName;
                            requirementResponse.ClientFavicon = patnerData1.Logo;
                            requirementResponse.PartnerCode = r.OrgCode;
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
                var applicantData = await _resourcesRepository.GetApplicationsPerRequirementIdAsyncV2(requirementId,request.VendorCode,request.Role);
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
                            StatusName = EnumHelper.GetEnumDescription<RecruitmentStatus>(applicationItem.Status),
                            ApplicationDate = applicationItem.CreatedOn,
                            FirstName = benchMember.FirstName,
                            LastName = benchMember.LastName,
                            VendorOrgCode = benchMember.OrgCode,
                            ApplicationId=applicationItem.Id
                            
                        };
                        var data = await _matchRecordRepository.GetMatchScoreAsync(requirementItem.Id, applicationItem.ResourceId);
                        if (data != null)
                        {
                            applicationResponse.MatchingScore = data.MatchScore;
                        }
                        applicationResponse.CvData = await GetCvByIdAsync(applicationItem.ResourceId);
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
            if (!string.IsNullOrWhiteSpace(request.VendorCode))
            {
                listResponse = listResponse.Where(v => v.VendorOrgCode == request.VendorCode).ToList();
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


                        var filteredApplications = allApplications
                            .Where(app => request.Status == null || !request.Status.Any() || request.Status.Contains(app.Status))
                            .ToList();

                        foreach (var app in filteredApplications)
                        {
                            // Create a response object for each application
                            var requirementResponse = new CompanyRequirementResponse
                            {
                                ApplicationId =app.Id,
                                RequirementUniqueId = item.UniqueId,
                                RequirementId = item.Id,
                                Role = item.Title,
                                ClientCode = item.ClientCode,
                                Position = item.Positions,
                                ApplicationDate = item.CreatedOn,
                                OrgName = orgData.OrgName,
                                OrgLogo = orgData.Logo,
                                Status = app.Status,
                                StatusName = CommonFunctions.GetEnumDescription((RecruitmentStatus)app.Status)
                            };

                            var vendorDetails = await _usersRepository.GetUserByIdAsync(app.CreatedBy);
                            var vendorOrgData = await _organizationRepository.GetOrganizationData(vendorDetails.OrgCode);
                            requirementResponse.Comment = app.Comment;
                            requirementResponse.VendorOrgName = vendorOrgData.OrgName;
                            requirementResponse.VendorLogo = vendorOrgData.Logo;
                            requirementResponse.VendorOrgCode = vendorOrgData.OrgCode;
                            requirementResponse.ResourceId = app.ResourceId;
                            var benchData = await _benchRepository.GetBenchResponseByIdAsync(app.ResourceId);
                            var matchResult = await _matchRecordRepository.GetMatchScoreAsync(app.RequirementId, app.ResourceId);
                            if (matchResult != null)
                            {
                                requirementResponse.MatchingScore = matchResult.MatchScore;
                            }
  
                          
                                
                            var candidateDetails = benchData?.FirstOrDefault();

                            if (candidateDetails != null)
                            {
                                requirementResponse.FirstName = candidateDetails.FirstName;
                                requirementResponse.LastName = candidateDetails.LastName;
                                requirementResponse.CV = await GetCvByIdAsync(candidateDetails.Id); 
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
                if (!string.IsNullOrEmpty(request.RequirmentUniqueId))
                {
                    listResponse = listResponse
                        .Where(r => r.RequirementUniqueId == request.RequirmentUniqueId).ToList();
                     
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

        public async Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId, int roletype)
        {
            try
            {
                var response= await _requirementRepository.GetVendorsCountsAsync(orgCode, userId,roletype);

                if (roletype == 1)
                {
               
                    List<int>    RequirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(orgCode);
                    var sharedrequirement = await _requirementRepository.GetRequirementByIdAsync(RequirementVendorsId);
                    var publicReq = await _requirementRepository.GetPublicRequirementAsync(null, 3);             
                    sharedrequirement = sharedrequirement.Concat(publicReq).Where(v => v.Status == (int)RequirementsStatus.Open);
                  
                    
                    int numberOfPositions = sharedrequirement.Sum(v => v.Positions);
                    response.OpenPositions = numberOfPositions;
                    response.HotRequirements = sharedrequirement.Count(v => v.Hot);
                }
                else
                {
                   int count=await   _requirementRepository.GetRequirementCountByOrgCodeAsync(orgCode);
                    response.OpenPositions = count;
                    List<int> RequirementVendorsIds = await _requirementVendorsRepository.GetRequirementShareJobsAsync(orgCode);
                    var sharedrequirements = await _requirementRepository.GetRequirementByIdAsync(RequirementVendorsIds);
                    response.HotRequirements = sharedrequirements.Count(v => v.Hot);

                }
               
                return response;
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

        public async Task<VendorRequirementCount> GetRequirementCountsAsync(CompanyGraphRequest request)
        {
            try
            {
                var data = await _requirementRepository.GetRequirementCountAsync(request);

                return new VendorRequirementCount
                {
                    Open = data.Open,
                    Onhold = data.Onhold,
                    Closed = data.Closed
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<VendorRequirementCount> GetVendorRequirementCountsAsync(VendorGraphRequest request)
        {
            try
            {

                var obj = new VendorRequirementCount();
                List<int> RequirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);
                var sharedrequirement = await _requirementRepository.GetRequirementByIdAsync(RequirementVendorsId);
                var publicReq = await _requirementRepository.GetPublicRequirementAsync(null, 3);
                sharedrequirement = sharedrequirement.Concat(publicReq);
                DateTime startDate = request.StartDate.Date;
                DateTime endDate = request.EndDate.Date.AddDays(1).AddTicks(-1);

                var filteredShared = sharedrequirement
                    .Where(v => v.CreatedOn >= startDate && v.CreatedOn <= endDate)
                    .ToList();

                obj.Open = filteredShared.Count(v => v.Status == (int)RequirementsStatus.Open);
                obj.Closed = filteredShared.Count(v => v.Status==(int)RequirementsStatus.Closed);
                obj.Onhold = filteredShared.Count(v => v.Status == (int)RequirementsStatus.OnHold);
                return obj;
   
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
                var requirementIds = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);

                var sharedRequirements = (await _requirementRepository.GetRequirementByIdAsync(requirementIds)).ToList();
                var publicRequirements = await _requirementRepository.GetPublicRequirementAsync(null, 3);

                var allRequirements = sharedRequirements
                    .Concat(publicRequirements)
                    .Where(r => !r.IsDeleted) // Just in case
                    .ToList();

                var requirementIdsAll = allRequirements.Select(r => r.Id).ToList();

                var totalPlacementsDict = await _resourcesRepository.GetPlacementsGroupedByRequirementAsync(requirementIdsAll);


                var groupedData = allRequirements
                    .GroupBy(r => r.CreatedOn.ToString("ddd")) // Group only by WeekDay
                    .Select(g =>
                    {
                        var reqIds = g.Select(x => x.Id).ToList();
                        int totalPlacements = totalPlacementsDict
                            .Where(p => reqIds.Contains(p.Key))
                            .Sum(p => p.Value);

                        return new VendorGraphResponse
                        {
                            OrgCode = request.OrgCode, // Not applicable when grouping by day
                            WeekDay = g.Key,
                            TotalPositions = g.Sum(x => x.Positions),
                            TotalPlacements = totalPlacements
                        };
                    })
                    .OrderBy(x => GetWeekDayOrder(x.WeekDay))
                    .ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetDayWeekCountsAsync: {ex.Message}", ex);
            }
        }
        private int GetWeekDayOrder(string weekDay)
        {
            return weekDay switch
            {
                "Mon" => 1,
                "Tue" => 2,
                "Wed" => 3,
                "Thu" => 4,
                "Fri" => 5,
                "Sat" => 6,
                "Sun" => 7,
                _ => 8
            };
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
                        obj.FavIconURL = client.FaviconURL;
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
                        obj.FavIconURL = client.FavIconURL;
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
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    benchData = benchData
                        .Where(v => v.FirstName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)
                                 || v.LastName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                if (request.Availability != null && request.Availability.Any())
                {
                    benchData = benchData
                        .Where(v => request.Availability.Contains(v.Availability))
                        .ToList();
                }




                foreach (var item in benchData)
                {
                    dynamic obj = new ExpandoObject();

                    var matchScoreResult = await _matchRecordRepository.GetMatchScoreAsync(request.RequirementId, item.Id);
                    int id = await _matchRecordRepository.GetMatchApplicant(request.RequirementId, item.Id);
                    if (id > 0)
                    {
                        obj.IsApplied = true;
                    }
                    else
                    {
                        obj.IsApplied = false;

                    }
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

        public async Task<string> GetCvByIdAsync(int id)
        {
            var data = await _benchRepository.GetBenchResponseByIdAsync(id);
            var cv = data.FirstOrDefault();

            if (cv == null)
                return null;

            var avatar = await GetAvtarByIdAsync(id);
            var cvJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(cv.CV.ToString());

            if (!string.IsNullOrWhiteSpace(avatar))
            {
                cvJson["avatar"] = avatar;
            }

            return JsonConvert.SerializeObject(cvJson); // ✅ convert dictionary to string
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

                var empaneledOrgs = await _partnerVendorRelRepository.GetBenchResponseListByIdAsync(request.OrgCode);
                var empOrgCodes = empaneledOrgs
                    .Where(x => x.PartnerCode == request.OrgCode || x.VendorCode == request.OrgCode)
                    .Select(x => x.PartnerCode == request.OrgCode ? x.VendorCode : x.PartnerCode)
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
        public async Task<List<SimilerRequirementResponse>> GetSimilerRequirementsAsync(SimilerRequirmentequest request)
        {
            List<int> requirementVendorsId = await _requirementVendorsRepository.GetRequirementShareJobsAsync(request.OrgCode);
            var sharedRequirements = await _requirementRepository.GetRequirementByIdAsync(requirementVendorsId);

            var targetRequirement = await _requirementRepository
                .GetRequirementByRequirementIdAsync(Convert.ToInt32(request.RequirmentId));

            if (targetRequirement == null) return new List<SimilerRequirementResponse>();

            var targetSkills = await _skillRequirementMappingRepository
                .GetSkillRequirementMappingAsync(targetRequirement.Id);

            var skillIds = targetSkills.Select(x => x.SkillId).Distinct().ToList();
            if (!skillIds.Any()) return new List<SimilerRequirementResponse>();

            var similarRequirementIds = await _skillRequirementMappingRepository
                .GetRequirementIdsBySkillMatchAsync(skillIds, targetRequirement.Id);

            if (!similarRequirementIds.Any()) return new List<SimilerRequirementResponse>();

       
            var sharedRequirementIds = sharedRequirements.Select(x => x.Id).ToList();
            var filteredIds = similarRequirementIds.Intersect(sharedRequirementIds).ToList();

            if (!filteredIds.Any()) return new List<SimilerRequirementResponse>();

            var pagedIds = filteredIds
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var requirements = await _requirementRepository.GetRequirementByIdAsync(pagedIds);

            var responseList = new List<SimilerRequirementResponse>();
            foreach (var req in requirements)
            {
                var candidateCount = await _matchRecordRepository.GetMatchingCountByRequirementId(req.Id);
                responseList.Add(new SimilerRequirementResponse
                {
                    Id = req.Id,
                    Title = req.Title,
                    OrgCode = req.OrgCode,
                    Description = req.Description,
                    Positions = req.Positions,
                    UniqueId = req.UniqueId,
                    MatchingCandidate = candidateCount.Count()
                });
            }

            return responseList;
        }
        public async Task<ActionMessageResponse> GetSharedContractsAsync(SharedContractsRequest request)
        {
            try
            {
                var records = await _resourcesRepository.GetSharedContractsAsync(request);


           

                var paginatedRecords = records
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new ActionMessageResponse
                {
                    Success = true,
                    Message = "Shared contracts fetched successfully.",
                    Content = new VendorContractResponse
                    {
                        Records = paginatedRecords,
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




    }
}
