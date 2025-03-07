namespace VendersCloud.Business.Service.Concrete
{
    public class OrgLocationService : IOrgLocationService
    {
        private readonly IOrgLocationRepository _orgLocationRepository;
        public OrgLocationService(IOrgLocationRepository orgLocationRepository)
        {
            _orgLocationRepository = orgLocationRepository;
        }

        public async Task<bool> UpsertLocation(OrgLocation location)
        {
            try
            {
                if (location == null)
                {
                    return false;
                }
                var response = await _orgLocationRepository.UpsertLocation(location);
                return response;
            }
            catch (Exception ex) {
                return false;
            }
        }

        public async Task<List<OrgLocation>> GetOrgLocation(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode)) {
                    return null;
                    }

                var response= await _orgLocationRepository.GetOrgLocation(orgCode);
                return response;
            }
            catch (Exception ex) { 
            return null;
            }
        }
    }
}
