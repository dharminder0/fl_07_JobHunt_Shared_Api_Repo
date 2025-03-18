using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class RequirementService : IRequirementService
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IBenchRepository _benchRepository;
        public RequirementService(IRequirementRepository requirementRepository, IClientsRepository clientsRepository,IResourcesRepository resourcesRepository,IBenchRepository benchRepository)
        {
            _requirementRepository = requirementRepository;
            _clientsRepository = clientsRepository;
            _resourcesRepository = resourcesRepository;
            _benchRepository = benchRepository;
        }

        public async Task<ActionMessageResponse> RequirmentUpsertAsync(RequirementRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.OrgCode)|| string.IsNullOrEmpty(request.UserId))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                var uniqueId = Guid.NewGuid().ToString().Substring(0, 12);
                var response = await _requirementRepository.RequirementUpsertAsync(request, uniqueId);
                if (response != null)
                {
                    var res = Convert.ToInt64(response);
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Submitted Successfully!! ", Content = uniqueId };
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
                            requirementResponse.ClientLogo = orgData.LogoURL;
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


                var response = await _requirementRepository.GetRequirementListByIdAsync(requirementId);

                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var orgData = await _clientsRepository.GetClientsByClientCodeAsync(item.ClientCode);
                        if (orgData != null)
                        {
                            res.ClientName = orgData.ClientName;
                            res.ClientLogo = orgData.LogoURL;
                        }

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

                var res = new List<RequirementResponse>();
                var response = await _requirementRepository.GetRequirementByOrgCodeAsync(orgCode);

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
                            requirementResponse.ClientLogo = orgData.LogoURL;
                        }

                        res.Add(requirementResponse);
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
                if (string.IsNullOrEmpty(request.OrgCode) || string.IsNullOrEmpty(request.UserId))
                {
                    throw new Exception("OrgCode is Mandatory!! ");
                }
                int place,Applicants = 0;
                var requirements = await _requirementRepository.GetRequirementsListAsync(request);
                var visibleRequirements = await _requirementRepository.GetRequirementsListByVisibilityAsync(request);
                var allRequirements = requirements.Concat(visibleRequirements).ToList();
                var totalRecords = allRequirements.Count;
                var paginatedRequirements = allRequirements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                TotalApplicantsRequest ApplicantSearch= new TotalApplicantsRequest();
                
                var requirementsResponseList = new List<RequirementResponse>();
                foreach (var r in paginatedRequirements)
                {
                    ApplicantSearch.RequirementUniqueId = r.UniqueId;
                    ApplicantSearch.Status = 8;
                    Applicants = await _resourcesRepository.GetTotalApplicationsPerRequirementIdAsync(r.Id);
                    place = await GetTotalApplicantsAsync(ApplicantSearch);
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
                        Applicants= Applicants,
                        StatusName = System.Enum.GetName(typeof(RequirementsStatus), r.Status),
                        CreatedOn = r.CreatedOn,
                        UpdatedOn = r.UpdatedOn,
                        CreatedBy = r.CreatedBy,
                        UpdatedBy = r.UpdatedBy,
                        IsDeleted = r.IsDeleted,
                        UniqueId = r.UniqueId
                    };

                    var orgData = await _clientsRepository.GetClientsByClientCodeAsync(r.ClientCode);
                    if (orgData != null)
                    {
                        requirementResponse.ClientName = orgData.ClientName;
                        requirementResponse.ClientLogo = orgData.LogoURL;
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
                int requirementId=0;
                if(request == null|| string.IsNullOrEmpty(request.RequirementUniqueId))
                {
                    throw new Exception("Enter Valid Inputs");
                }
                var requiementData = await _requirementRepository.GetRequirementListByIdAsync(request.RequirementUniqueId);
                if (requiementData != null)
                {
                    foreach (var item in requiementData)
                    {
                        requirementId=item.Id;
                    }
                }
                var applicantData = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(requirementId, request.Status);
                var totalApplicants = applicantData.Count;
                return totalApplicants;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ApplicationListResponse>> GetApplicantsListByRequirementIdAsync(string requirementUniqueId)
        {
            if (string.IsNullOrEmpty(requirementUniqueId))
            {
                throw new ArgumentException("Requirement ID cannot be null or empty");
            }

            var listResponse = new List<ApplicationListResponse>();

            try
            {
                var requirementData = await _requirementRepository.GetRequirementListByIdAsync(requirementUniqueId);
                if (requirementData == null)
                {
                    return listResponse;
                }

                foreach (var requirementItem in requirementData)
                {
                    var applicationData = await _resourcesRepository.GetApplicationsPerRequirementIdAsync(requirementItem.Id);
                    if (applicationData == null) continue;

                    foreach (var applicationItem in applicationData)
                    {
                        var applicationResponse = new ApplicationListResponse
                        {
                            Title = requirementItem.Title,
                            RequirementId = requirementItem.Id,
                            Status = applicationItem.Status,
                            StatusName = System.Enum.GetName(typeof(ApplyStatus), applicationItem.Status),
                            ApplicationDate= applicationItem.CreatedOn
                        };

                        var benchData = await _benchRepository.GetBenchResponseByIdAsync(applicationItem.ResourceId);
                        if (benchData != null && benchData.Any())
                        {
                            var benchMember = benchData.First();
                            applicationResponse.FirstName = benchMember.FirstName;
                            applicationResponse.LastName = benchMember.LastName;
                        }

                        listResponse.Add(applicationResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return listResponse;
        }

    }
}
