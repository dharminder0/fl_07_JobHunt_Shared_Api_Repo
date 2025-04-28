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
        Task<List<PartnerVendorRel>> GetAllAsync();
        Task<bool> UpdatePartnerVendorRelByIdAsync(int id, PartnerVendorRel updatedEntity);
        Task<int> AddPartnerVendorRelAsync(PartnerVendorRel entity);
        Task<List<PartnerVendorRel>> GetByPartnerIdAsync(string partnerCode, string vendorCode);
        Task<bool> ManagePartnerStatusAsync(ManageRelationshipStatusRequest manageRelationship);


    }
 
}
