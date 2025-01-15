using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository= userRepository;

        }

        public async Task<IEnumerable<User>> GetAllUsersInfo()
        {
            try
            {
                var result = await _userRepository.GetAllUsersInfo();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}
