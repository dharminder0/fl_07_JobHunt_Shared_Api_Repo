using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class OrgProfilesService : IOrgProfilesService
    {
        private readonly IOrgProfilesRepository _orgProfilesRepository;
        public OrgProfilesService(IOrgProfilesRepository orgProfilesRepository)
        {
            _orgProfilesRepository = orgProfilesRepository;
        }
        public async Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode) || profileId < 0)
                {
                    return false;
                }
                var response = await _orgProfilesRepository.AddOrganizationProfileAsync(orgCode, profileId);
                return response;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<PaginationDto<Organization>> SearchOrganizationsDetails(SearchRequest request)
        {
            try
            {
                return await _orgProfilesRepository.SearchOrganizationsDetails(request);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }

}
