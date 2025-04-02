namespace VendersCloud.Business.Service.Abstract
{
    public interface IRequirementService
    {
        Task<ActionMessageResponse> RequirmentUpsertAsync(RequirementRequest request);
        Task<ActionMessageResponse> DeleteRequirementAsync(int requirementId, string orgCode);
        Task<List<RequirementResponse>> GetRequirementListAsync();
        Task<RequirementResponse> GetRequirementListByIdAsync(string requirementId);
        Task<ActionMessageResponse> UpdateStatusByIdAsync(int requirementId, int status);
        Task<List<RequirementResponse>> GetRequirementByOrgCodeAsync(string orgCode);
        Task<PaginationDto<RequirementResponse>> SearchRequirementAsync(SearchRequirementRequest request);
        Task<int> GetTotalApplicantsAsync(TotalApplicantsRequest request);
        Task<PaginationDto<ApplicationListResponse>> GetApplicantsListByRequirementIdAsync(GetApplicantsByRequirementRequest request);
        Task<PaginationDto<CompanyRequirementResponse>> GetRequirementListByOrgCode(CompanyRequirementSearchRequest request);
        Task<CompanyDashboardCountResponse> GetCountsAsync(string orgCode);
        Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId);
        Task<List<CompanyGraphResponse>> GetDayWeekCountsAsync(CompanyGraphRequest request);
        Task<dynamic> GetRequirementCountsAsync(CompanyGraphRequest request);
        Task<dynamic> GetVendorRequirementCountsAsync(VendorGraphRequest request);
        Task<List<VendorGraphResponse>> GetVendorDayWeekCountsAsync(VendorGraphRequest request);
        Task<ActionMessageResponse> HotRequirementUpsertAsync(HotRequirementRequest request);
        Task<PaginationDto<dynamic>> GetHotRequirementAsync(GetHotRequirmentRequest request);
    }
}
