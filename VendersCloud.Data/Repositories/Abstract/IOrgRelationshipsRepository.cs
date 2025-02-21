using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgRelationshipsRepository: IBaseRepository<OrgRelationships>
    {
        Task<bool> AddOrgRelationshipDataAsync(string orgCode, string relatedOrgCode, string relationshipType, int status, int createdBy);
        Task<bool> ManageRelationshipStatusAsync(int orgRelationshipId, int status);
        Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request);
    }
}
