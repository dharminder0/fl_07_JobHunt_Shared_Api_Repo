using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

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
    }
}
