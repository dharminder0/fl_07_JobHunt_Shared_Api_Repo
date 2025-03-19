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
        Task<List<ApplicationListResponse>> GetApplicantsListByRequirementIdAsync(string requirementUniqueId);
        Task<PaginationDto<CompanyRequirementResponse>> GetRequirementListByOrgCode(CompanyRequirementSearchRequest request);
    }
}
