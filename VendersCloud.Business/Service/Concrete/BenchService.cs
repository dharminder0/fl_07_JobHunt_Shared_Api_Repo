using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class BenchService : IBenchService
    {
        private readonly IBenchRepository _benchRepository;
        public BenchService(IBenchRepository benchRepository)
        {
            _benchRepository = benchRepository;
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
                        AvailabilityName = Enum.GetName(typeof(BenchAvailability), item.Availability),
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
    }
}

