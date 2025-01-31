using VendersCloud.Business.Entities.Dtos;
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
                        RegistrationDto registration= new RegistrationDto();
                        registration.UserId = data;
                        registration.CompanyCode = orgCode;
                        registration.UserEmail = request.Email;
                        return new ActionMessageResponse { Success = true, Message = "New Client Registered Successfully!!", Content = registration };
                    }
                    return new ActionMessageResponse { Success = false, Message = "Not Added",Content="" };
                }
                return new ActionMessageResponse { Success = false, Message = "Not Added", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = "Not Added" , Content = "" };

            }
        }

        public async Task<ActionMessageResponse> LoginUserAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new ActionMessageResponse { Success = false, Message = "Inputs cannot be Empty/Null", Content = "" };
                }
                var dbUser = await _usersRepository.GetUserByEmailAsync(request.Email);
                if(dbUser!=null && dbUser.PasswordSalt != null)
                {
                    var saltBytes = dbUser.PasswordSalt;
                    string salt = Convert.ToBase64String(saltBytes);
                    var hashedPassword = Hasher.HashPassword(salt, request.Password);
                    if(hashedPassword== dbUser.Password)
                    {
                        LoginResponseDto login = new LoginResponseDto();
                        login.UserId = dbUser.Id.ToString();
                        login.Email = dbUser.UserName;
                        login.OrgCode = dbUser.OrgCode;

                        return new ActionMessageResponse { Success = true, Message = "Login SuccessFull!!", Content = login };
                    }
                }
                return new ActionMessageResponse { Success = false, Message = "Not Valid User!!", Content = "" };
            }
            catch(Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = "Enter Valid Inputs!!!" };
            }
        }
    }
}
