namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgRelationshipsRepository: IBaseRepository<OrgRelationships>
    {
        Task<bool> AddOrgRelationshipDataAsync(string orgCode, string relatedOrgCode, string relationshipType, int status, int createdBy);
        Task<bool> ManageRelationshipStatusAsync(int orgRelationshipId, int status);
        Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request);
        Task<IEnumerable<OrgRelationships>> GetBenchResponseListByIdAsync(string orgCode);
        Task<List<OrgRelationships>> GetOrgRelationshipsListAsync(string orgCode);
    }
}
