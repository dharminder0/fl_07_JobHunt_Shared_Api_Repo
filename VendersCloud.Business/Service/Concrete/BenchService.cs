using System.ComponentModel;
using System.Reflection;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class BenchService : IBenchService
    {
        private readonly IBenchRepository _benchRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IRequirementRepository _requirementsRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public BenchService(IBenchRepository benchRepository, IResourcesRepository resourcesRepository, IRequirementRepository requirementsRepository, IOrganizationRepository organizationRepository)
        {
            _benchRepository = benchRepository;
            _resourcesRepository = resourcesRepository;
            _requirementsRepository = requirementsRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(benchRequest.OrgCode) || string.IsNullOrEmpty(benchRequest.FirstName) || string.IsNullOrEmpty(benchRequest.Email))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Enter Valid Inputs",
                        Content = ""
                    };
                }
                var res = await _benchRepository.UpsertBenchMembersAsync(benchRequest);
                if (res)
                    return new ActionMessageResponse()
                    {
                        Success = true,
                        Message = "Bench Member added",
                        Content = ""
                    };
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
                throw new Exception("Enter Valid Inputs");
            }
            try
            {
                var response = await _benchRepository.GetBenchListBySearchAsync(request);
                var totalRecords = response.Count;
                var paginatedResponse = response.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
                var BenchAvailability = new List<BenchResponse>();
                foreach (var item in paginatedResponse)
                {
                    var benchresponse = new BenchResponse
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Title = item.Title,
                        Email = item.Email,
                        Phone = item.Phone,
                        Linkedin = item.Linkedin,
                        CV = item.CV,
                        OrgCode = item.OrgCode,
                        Availability = item.Availability,
                        AvailabilityName = GetEnumDescription((BenchAvailability)item.Availability),
                        CreatedOn = item.CreatedOn,
                        UpdatedOn = item.UpdatedOn,
                        CreatedBy = item.CreatedBy,
                        UpdatedBy = item.UpdatedBy,
                        IsDeleted = item.IsDeleted,
                    };
                    BenchAvailability.Add(benchresponse);
                }

                return new PaginationDto<BenchResponse>
                {
                    Count = totalRecords,
                    Page = request.Page,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    List = BenchAvailability
                };
            }
            catch (Exception ex)
            {
                throw ex;
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
                if (requirementdata != null) {
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

                
                if (!string.IsNullOrEmpty(request.UserId) && int.TryParse(request.UserId, out var id))
                {
                    query = query.Where(a => a.CreatedBy == id);
                }

               
                if (request.Status > 0)
                {
                    query = query.Where(a => a.Status == request.Status);
                }

                
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    query = query.Where(a =>
                        _benchRepository.GetBenchResponseListByIdAsync(a.ResourceId)
                            .Result.Any(r =>
                                (!string.IsNullOrEmpty(r.FirstName) && r.FirstName.Contains(request.SearchText)) ||
                                (!string.IsNullOrEmpty(r.LastName) && r.LastName.Contains(request.SearchText))
                            )
                    );
                }

                
                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var pagedResults = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

                
                foreach (var data in pagedResults)
                {
                    ApplicantsSearchResponse searchResponse = new ApplicantsSearchResponse
                    {
                        Status = data.Status,
                        StatusName = GetEnumDescription((ApplyStatus)data.Status),
                        ApplicationDate = data.CreatedOn
                    };

                   
                    var requirementData = await _requirementsRepository.GetRequirementByIdAsync(data.RequirementId);
                    if (requirementData?.FirstOrDefault() is { } odata)
                    {
                        var orgData = await _organizationRepository.GetOrganizationData(odata.OrgCode);
                        if (orgData != null && !string.IsNullOrEmpty(request.ClientOrgName) && orgData.OrgName == request.ClientOrgName)
                        {
                            searchResponse.ClientOrgName = orgData.OrgName;
                            searchResponse.ClientOrgLogo = orgData.Logo;
                        }

                        searchResponse.Requirement = odata.Title;
                    }

                   
                    var resourceData = await _benchRepository.GetBenchResponseListByIdAsync(data.ResourceId);
                    var resource = resourceData?.FirstOrDefault();
                    if (resource != null)
                    {
                        searchResponse.FirstName = resource.FirstName;
                        searchResponse.LastName = resource.LastName;
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
                throw new Exception(ex.Message);
            }
        }



        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

