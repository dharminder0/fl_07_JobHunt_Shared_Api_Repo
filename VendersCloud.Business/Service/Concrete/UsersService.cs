using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Common.Utils;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class UsersService:IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IOrganizationService _organizationService;
        public UsersService(IUsersRepository usersRepository, IOrganizationService organizationService)
        {
            _usersRepository = usersRepository;
            _organizationService = organizationService;
        }

        public async Task<ActionMessageResponse>RegisterNewUserAsync(RegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)|| string.IsNullOrEmpty(request.CompanyName)||string.IsNullOrEmpty(request.Email))
                {
                    return new ActionMessageResponse { Success = false, Message = "Values can't be null/empty" };
                }
                var orgCode = await _organizationService.RegisterNewOrganizationAsync(request);
                if (orgCode != null)
                {
                    string salt = Hasher.GenerateSalt();
                    byte[] saltBytes = Convert.FromBase64String(salt);
                    var hashedPassword = Hasher.HashPassword(salt, request.Password);
                    var data = await _usersRepository.InsertUserAsync(request, hashedPassword, saltBytes, orgCode);
                    if (data != null)
                    {
                        return new ActionMessageResponse { Success = true, Message = "New Client Registered Successfully!!", Content = data };
                    }
                    return new ActionMessageResponse { Success = false, Message = "Not Added" };
                }
                return new ActionMessageResponse { Success = false, Message = "Not Added" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = "Not Added" };

            }
        }
    }
}
