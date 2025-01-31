using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUsersService
    {
        Task<ActionMessageResponse> RegisterNewUserAsync(RegistrationRequest request);
    }
}
