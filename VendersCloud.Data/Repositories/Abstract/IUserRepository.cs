using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserRepository :IDataRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersInfoAsync();
        Task<User> UserLoginAsync(UserLoginRequestModel loginRequest);
        Task<string> UpsertAsync(UserSignUpRequestModel usersign, string userId);
        Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo);
        Task<IEnumerable<User>> GetUserDetailsByUserIdAsync(string userId);
    }
}
