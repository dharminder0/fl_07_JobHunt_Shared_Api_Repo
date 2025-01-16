using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersInfo();
        Task<UserLoginResponseModel> UserLogin(UserLoginRequestModel loginRequest);
        Task<string> UserSignUp(string companyName, string email, string password);
    }
}
