namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementVendorsRepository :IBaseRepository<RequirementVendors>
    {
        Task<bool> AddRequirementVendorsDataAsync(int requirementId, string orgCode);
        Task<List<int>> GetRequirementShareJobsAsync(string orgCode);
        Task<List<int>> GetRequirementShareJobsAsyncV2(List<string> orgCode);
    }
}
