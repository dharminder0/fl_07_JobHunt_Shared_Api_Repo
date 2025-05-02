
namespace VendersCloud.Business.Service.Abstract
{
    public interface IBenchService
    {
        Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest);
        Task<List<Resources>> GetBenchListAsync(string orgCode);
        Task<PaginationDto<BenchResponse>> GetBenchListBySearchAsync(BenchSearchRequest request);
        Task<ActionMessageResponse> UpsertApplicants(ApplicationsRequest request);
        Task<PaginationDto<ApplicantsSearchResponse>> GetSearchApplicantsList(ApplicantsSearchRequest request);
        Task<PaginationDto<OrgActivePositionsResponse>> GetActiveVacanciesByOrgCodeAsync(CompanyActiveClientResponse request);
        Task<PaginationDto<dynamic>> GetTopVendorsListAsync(CompanyActiveClientResponse request);
        Task<PaginationDto<OrgActivePositionsResponse>> GetActiveVacanciesByUserIdAsync(VendorActiveClientResponse request);
        Task<dynamic> GetCountTechStackByOrgCodeAsync(string orgCode);
        Task<dynamic> GetCvByIdAsync(int id);
        Task<bool> UpsertCvAvtarAsync(UpsertCvAvtarRequest request);
        Task<string> GetAvtarByIdAsync(int benchId);
        Task<List<dynamic>> GetBenchMatchResultAsync(BenchMatchRecord request);
        Task<ActionMessageResponse> GetVendorContractsAsync(VendorContractRequest request);
    }
}
