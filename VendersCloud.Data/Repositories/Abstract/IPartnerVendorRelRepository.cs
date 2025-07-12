using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendersCloud.Data.Repositories.Concrete;

namespace VendersCloud.Data.Repositories.Abstract
{
    public  interface IPartnerVendorRelRepository : IBaseRepository<PartnerVendorRel>
    {
        Task<PartnerVendorRel> GetByIdAsync(int id);
     
        Task<bool> UpdatePartnerVendorRelByIdAsync(int id, PartnerVendorRel updatedEntity);
      
        Task<List<PartnerVendorRel>> GetByPartnerIdAsync(string partnerCode, string vendorCode);
        Task<bool> ManagePartnerStatusAsync(ManageRelationshipStatusRequest manageRelationship);
        Task<PartnerVendorRel> ManagePartnerStatusAsync(string partnerCode, string vendorCode);
        Task<List<PartnerVendorRel>>  GetOrgRelationshipsListAsync(string orgCode);
        Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request);
        Task<List<string>> GetAllVendorCodeAsync(string partnerCode);
        Task<IEnumerable<PartnerVendorRel>> GetBenchResponseListByIdAsync(string orgCode);
        Task<int> UpsertPartnerVendorRelAsync(PartnerVendorRel entity);

    }
 
}
