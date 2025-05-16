namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementRepository :IBaseRepository<Requirement>
    {
        Task<Requirement> RequirementUpsertAsync(RequirementRequest request, string uniqueId);
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
        Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId,int roleId);
        Task<List<dynamic>> GetActivePositionsByOrgCodeAsync(string orgCode, string userId);
        Task<List<dynamic>> GetOrgTotalPlacementAndRequirementIdAsync(CompanyGraphRequest request);
        Task<VendorRequirementCount> GetRequirementCountAsync(CompanyGraphRequest request);
        Task<VendorRequirementCount> GetVendorRequirementCountAsync(VendorGraphRequest request);
        Task<List<dynamic>> GetVendorTotalPlacementAndRequirementIdAsync(VendorGraphRequest request);
        Task<bool> UpdateHotByIdAsync(string requirementUniqueId, int hot);
        Task<dynamic> GetCountTechStackByOrgCodeAsync(string orgCode);
        Task<Requirement> GetRequirementByRequirementIdAsync(int requirementId);
        Task<int> GetRequirementCountByOrgCodeAsync(string orgCode);
        Task<List<Requirement>> GetPublicRequirementAsync(List<string> orgCode, int visibility);
        Task<int> GetRequirementCountByOrgCodeAsyncV2(string orgCode,string clientCode);
    }
}
