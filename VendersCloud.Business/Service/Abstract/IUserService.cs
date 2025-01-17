using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersInfoAsync();
        Task<UserLoginResponseModel> UserLoginAsync(UserLoginRequestModel loginRequest);
        Task<string> UserSignUpAsync(UserSignUpRequestModel usersign);
    }
}
