namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IResourcesRepository : IBaseRepository<Resources>
    {
        Task<bool> UpsertApplicants(ApplicationsRequest request, int Id);
        Task<List<Applications>> GetApplicationsList();
        Task<List<int>> GetApplicationsPerRequirementIdAsync(int requirementId, int status);
        Task<int> GetTotalApplicationsPerRequirementIdAsync(int requirementId);
    }
}
