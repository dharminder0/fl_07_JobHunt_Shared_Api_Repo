namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IResourcesRepository : IBaseRepository<Resources>
    {
        Task<bool> UpsertApplicants(ApplicationsRequest request, int Id);
        Task<List<Applications>> GetApplicationsList();
        Task<List<dynamic>> GetApplicationsPerRequirementIdAsync(int requirementId, int status);
        Task<int> GetTotalApplicationsPerRequirementIdAsync(int requirementId);
        Task<List<Applications>> GetApplicationsPerRequirementIdAsync(int requirementId);
        Task<int> GetTotalPlacementsAsync(List<int> requirementIds);
        Task<int> GetTotalPlacementsByUserIdsAsync(List<int> userId);
        Task<List<VendorDetailDto>> GetContractsByTypeAsync(VendorContractRequest request);
        Task<Dictionary<int, int>> GetPlacementsGroupedByRequirementAsync(List<int> requirementIds);
        Task<List<VendorDetailDto>> GetSharedContractsAsync(SharedContractsRequest request);
    }
}
