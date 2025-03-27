namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementRepository :IBaseRepository<Requirement>
    {
        Task<string> RequirementUpsertAsync(RequirementRequest request, string uniqueId);
        Task<bool> DeleteRequirementAsync(int requirementId, string orgCode);
        Task<List<Requirement>> GetRequirementListAsync();
        Task<List<Requirement>> GetRequirementListByIdAsync(string requirementId);
        Task<bool> RequirementUpsertV2Async(RequirementDto request, string uniqueId);
        Task<bool> UpdateStatusByIdAsync(int requirementId, int status);
        Task<List<Requirement>> GetRequirementByOrgCodeAsync(string orgCode);
        Task<List<Requirement>> GetRequirementsListAsync(SearchRequirementRequest request);
        Task<List<Requirement>> GetRequirementsListByVisibilityAsync(SearchRequirementRequest request);
        Task<IEnumerable<Requirement>> GetRequirementByIdAsync(List<int> requirementId);
        Task<List<Requirement>> GetRequirementByUserIdAsync(List<int> UserId);
        Task<CompanyDashboardCountResponse> GetCountsAsync(string orgCode);
        Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId);
        Task<List<dynamic>> GetActivePositionsByOrgCodeAsync(string orgCode);
        Task<List<dynamic>> GetOrgTotalPlacementAndRequirementIdAsync(CompanyGraphRequest request);
        Task<dynamic> GetRequirementCountAsync(CompanyGraphRequest request);
    }
}
