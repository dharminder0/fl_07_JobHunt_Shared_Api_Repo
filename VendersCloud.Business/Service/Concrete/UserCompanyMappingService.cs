using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class UserCompanyMappingService : IUserCompanyMappingService
    {
        private readonly IUserCompanyMappingRepository _userCompanyMappingRepository;
        public UserCompanyMappingService(IUserCompanyMappingRepository userCompanyMappingRepository)
        {
            _userCompanyMappingRepository = userCompanyMappingRepository;
        }

        public async Task<UserCompanyMapping> GetMappingsByUserIdAsync(string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var mapping = await _userCompanyMappingRepository.GetMappingsByUserIdAsync(userId);
                    return mapping;

                }
                else
                {
                    throw new Exception("Userid can't be blank");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddMappingAsync(string userId, string companyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(companyCode))
                {
                    var res = await _userCompanyMappingRepository.AddMappingAsync(userId, companyCode);
                    return res;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserCompanyMapping>> GetMappingsByCompanyCodeAsync(string companyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyCode))
                {
                    var mapping = await _userCompanyMappingRepository.GetMappingsByCompanyCodeAsync(companyCode);
                    return mapping;

                }
                else
                {
                    throw new Exception("companyCode can't be blank");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
