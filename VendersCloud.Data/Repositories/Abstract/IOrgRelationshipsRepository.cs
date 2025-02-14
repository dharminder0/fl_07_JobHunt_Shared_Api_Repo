using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgRelationshipsRepository: IBaseRepository<OrgRelationships>
    {
        Task<bool> AddOrgRelationshipDataAsync(string orgCode, string relatedOrgCode, string relationshipType, int status, int createdBy);
        Task<bool> ManageRelationshipStatusAsync(int orgRelationshipId, int status);
    }
}
