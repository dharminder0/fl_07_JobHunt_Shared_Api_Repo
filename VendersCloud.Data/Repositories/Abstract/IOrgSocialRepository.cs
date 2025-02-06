using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IOrgSocialRepository:IBaseRepository<OrgSocial>
    {
        Task<bool> UpsertSocialProfile(OrgSocial social);
    }
}
