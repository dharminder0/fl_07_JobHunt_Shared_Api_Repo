using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;

namespace VendersCloud.Business.Service.Concrete
{
    public class BenchService : IBenchService
    {
        private readonly IBenchRepository _benchRepository;
        private readonly IResourcesRepository _resourcesRepository;
        public BenchService(IBenchRepository benchRepository, IResourcesRepository resourcesRepository)
        {
            _benchRepository = benchRepository;
            _resourcesRepository = resourcesRepository;
        }

        public async Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest)
        {
            try
            {
                if(string.IsNullOrEmpty(benchRequest.OrgCode)|| string.IsNullOrEmpty(benchRequest.FirstName)||string.IsNullOrEmpty(benchRequest.Email))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Enter Valid Inputs",
                        Content = ""
                    };
                }
                var res= await _benchRepository.UpsertBenchMembersAsync(benchRequest);
                if(res)
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
            catch (Exception ex) {
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
            catch (Exception ex) {
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
                var paginatedResponse= response.Skip((request.Page-1)* request.PageSize).Take(request.PageSize).ToList();
                var BenchAvailability = new List<BenchResponse>();
                foreach (var item in paginatedResponse)
                {
                    var benchresponse = new BenchResponse {
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
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<ActionMessageResponse> UpsertApplicants(ApplicationsRequest request)
        {
            try
            {
                if (request.ResourceId == null || !request.ResourceId.Any() || request.RequirementId <= 0)
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Invalid input: ResourceId cannot be null or empty and RequirementId must be positive.",
                        Content = ""
                    };
                }

             
                var res = await _resourcesRepository.UpsertApplicants(request);
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
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

