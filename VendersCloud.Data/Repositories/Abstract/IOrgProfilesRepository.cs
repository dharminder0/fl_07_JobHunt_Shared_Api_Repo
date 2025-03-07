﻿namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgProfilesRepository:IBaseRepository<OrgProfiles>
    {
        Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId);
        Task<PaginationDto<Organization>> SearchOrganizationsDetails(SearchRequest request);
        Task<List<OrgProfiles>> GetOrgProfilesByOrgCodeAsync(string orgCode);
    }
}
