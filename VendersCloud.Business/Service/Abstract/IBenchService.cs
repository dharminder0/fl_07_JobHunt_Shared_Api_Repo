using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IBenchService
    {
        Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest);
        Task<List<Resources>> GetBenchListAsync(string orgCode);
        Task<PaginationDto<BenchResponse>> GetBenchListBySearchAsync(BenchSearchRequest request);
    }
}
