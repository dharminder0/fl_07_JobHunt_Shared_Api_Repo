using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementRepository :IBaseRepository<Requirement>
    {
        Task<bool> RequirementUpsertAsync(RequirementRequest request);
        Task<bool> DeleteRequirementAsync(int requirementId, string orgCode);
        Task<List<Requirement>> GetRequirementListAsync();
    }
}
