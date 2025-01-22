using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersInfoAsync();
        Task<ActionMessageResponseModel> UserLoginAsync(UserLoginRequestModel loginRequest);
        Task<ActionMessageResponseModel> UserSignUpAsync(UserSignUpRequestModel usersign);
        Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo);

        Task<IEnumerable<User>> GetUserDetailsByUserIdAsync(string userId);
    }
}
