﻿using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementRepository :IBaseRepository<Requirement>
    {
        Task<bool> RequirementUpsertAsync(RequirementRequest request);
        Task<bool> DeleteRequirementAsync(int requirementId, string orgCode);
        Task<List<Requirement>> GetRequirementListAsync();
        Task<List<Requirement>> GetRequirementListByIdAsync(int requirementId);
        Task<bool> RequirementUpsertV2Async(RequirementDto request);
    }
}
