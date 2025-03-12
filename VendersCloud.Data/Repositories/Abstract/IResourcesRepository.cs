namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IResourcesRepository : IBaseRepository<Resources>
    {
        Task<bool> UpsertApplicants(ApplicationsRequest request);
    }
}
