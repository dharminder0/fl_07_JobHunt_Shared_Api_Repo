using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserRepository :IDataRepository<Users>
    {
        Task<IEnumerable<Users>> GetAllUsersInfoAsync();
        Task<Users> UserLoginAsync(UserLoginRequestModel loginRequest);
        Task<string> UpsertAsync(UserSignUpRequestModel usersign, string userId, string passwordSalt);
        Task<bool> UpdateRoleAsync(string userId, string roleType);
        Task<IEnumerable<Users>> GetUserDetailsByUserIdAsync(string userId);
        Task<IEnumerable<Users>> GetUserDetailsByRoleTypeAsync(string userId, string roletype);
        Task<IEnumerable<Users>> GetUserDetailsByRoleAsync(string roletype);
        Task<IEnumerable<Users>> GetUserByEmail(string email);
    }
}
