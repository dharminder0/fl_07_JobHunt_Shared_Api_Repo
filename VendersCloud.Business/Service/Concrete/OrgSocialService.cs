namespace VendersCloud.Business.Service.Concrete
{
    public class OrgSocialService: IOrgSocialService
    {
        private readonly IOrgSocialRepository _orgSocialRepository;
        public OrgSocialService(IOrgSocialRepository orgSocialRepository)
        {
            _orgSocialRepository = orgSocialRepository;
        }

        public async Task<bool> UpsertSocialProfile(OrgSocial social)
        {
            try
            {
                if (social == null) {
                    return false;
                }
                var response= await _orgSocialRepository.UpsertSocialProfile(social);
                return response;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<OrgSocial>> GetOrgSocialProfile(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode)) {
                    return null;
                }
                var response= await _orgSocialRepository.GetOrgSocialProfile(orgCode);
                return response;
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}
