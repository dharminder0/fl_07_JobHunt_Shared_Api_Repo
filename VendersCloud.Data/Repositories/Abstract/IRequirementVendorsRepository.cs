﻿namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IRequirementVendorsRepository :IBaseRepository<RequirementVendors>
    {
        Task<bool> AddRequirementVendorsDataAsync(int requirementId, string orgCode);
    }
}
