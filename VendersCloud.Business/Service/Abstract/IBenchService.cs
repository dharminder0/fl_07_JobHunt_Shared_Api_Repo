﻿
namespace VendersCloud.Business.Service.Abstract
{
    public interface IBenchService
    {
        Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest);
        Task<List<Resources>> GetBenchListAsync(string orgCode);
        Task<PaginationDto<BenchResponse>> GetBenchListBySearchAsync(BenchSearchRequest request);
    }
}
