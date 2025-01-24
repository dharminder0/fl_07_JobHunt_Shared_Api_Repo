using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetAllUsersInfoAsync();
        Task<ActionMessageResponseModel> UserLoginAsync(UserLoginRequestModel loginRequest);
        Task<ActionMessageResponseModel> UserSignUpAsync(UserSignUpRequestModel usersign);
        Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo);

        Task<IEnumerable<Users>> GetUserDetailsByUserIdAsync(string userId);
        Task<IEnumerable<Users>> GetUserDetailsByRoleTypeAsync(string userId, string roletype);
    }
}
