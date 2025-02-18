using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

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
    }
}
