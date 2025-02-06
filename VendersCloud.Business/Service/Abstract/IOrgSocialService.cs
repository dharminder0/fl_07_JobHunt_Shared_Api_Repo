using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrgSocialService
    {
        Task<bool> UpsertSocialProfile(OrgSocial social);
        Task<List<OrgSocial>> GetOrgSocialProfile(string orgCode);
    }
}
