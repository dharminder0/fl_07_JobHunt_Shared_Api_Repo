using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Common.Utils;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class UsersService:IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IOrganizationService _organizationService;
        private readonly IUserProfilesService _userProfilesService;
        public UsersService(IUsersRepository usersRepository, IOrganizationService organizationService, IUserProfilesService userProfilesService)
        {
            _usersRepository = usersRepository;
            _organizationService = organizationService;
            _userProfilesService= userProfilesService;
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
                    var userProfileRole = await _userProfilesService.GetProfileRole(dbUser.Id);
                    var companyData = await _organizationService.GetOrganizationDataAsync(dbUser.OrgCode);
                    string roleName = Enum.GetName(typeof(RoleType), userProfileRole);
                    if (hashedPassword== dbUser.Password)
                    {
                        LoginResponseDto login = new LoginResponseDto();
                        login.UserId = dbUser.Id.ToString();
                        login.Email = dbUser.UserName;
                        login.OrgCode = dbUser.OrgCode;
                        login.Role = roleName;
                        login.CompanyIcon = companyData.Logo;
                        login.CompanyName = companyData.OrgName;

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

        public async Task<ActionMessageResponse> DeleteUserAsync( string email, string organizationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(organizationCode))
                {
                    return new ActionMessageResponse { Success = false, Message = "Inputs cannot be Empty/Null", Content = "" };
                }
                var dbUser = await _usersRepository.GetUserByEmailAsync(email);
                if (dbUser == null) {
                    return new ActionMessageResponse { Success = false, Message = "User Not Found!!", Content = "" };
                }
                var response= await _usersRepository.DeleteUserByEmailAndOrgCodeAsync(email, organizationCode);
                if (response)
                {
                    return new ActionMessageResponse { Success = true, Message = "User Deleted Successfully", Content = "" };
                }
                return new ActionMessageResponse { Success = false, Message = "User Not Deleted Successfully", Content = "" };

            }
            catch (Exception ex) {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }

        }

        public async Task<ActionMessageResponse> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ActionMessageResponse { Success = false, Message = "Inputs cannot be Empty/Null", Content = "" };
                }
                var dbUser = await _usersRepository.GetUserByEmailAsync(email);
                if (dbUser == null)
                {
                    return new ActionMessageResponse { Success = false, Message = "User Not Found!!", Content = "" };
                }
                UsersDto userdto = new UsersDto
                {
                    Id = dbUser.Id,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    UserName = dbUser.UserName,
                    OrgCode = dbUser.OrgCode,
                    Gender = dbUser.Gender,
                    IsVerified = dbUser.IsVerified,
                    ProfileAvatar = dbUser.ProfileAvatar,
                    CreatedOn = dbUser.CreatedOn,
                    UpdatedOn = dbUser.UpdatedOn,
                    LastLoginTime = dbUser.LastLoginTime,
                    IsDeleted = dbUser.IsDeleted
                };
                return new ActionMessageResponse { Success = false, Message = "User  Found!!", Content = userdto };
            }
            catch (Exception ex) {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<List<UsersDto>> GetAllUserAsync()
        {
            try
            {
                List<UsersDto> userDtoList = new List<UsersDto>();
                var response = await _usersRepository.GetAllUserAsync();
                if (response != null)
                {
                    foreach (var dbUser in response)
                    {
                        UsersDto userDto = new UsersDto()
                        {
                            Id = dbUser.Id,
                            FirstName = dbUser.FirstName,
                            LastName = dbUser.LastName,
                            UserName = dbUser.UserName,
                            OrgCode = dbUser.OrgCode,
                            Gender = dbUser.Gender,
                            IsVerified = dbUser.IsVerified,
                            ProfileAvatar = dbUser.ProfileAvatar,
                            CreatedOn = dbUser.CreatedOn,
                            UpdatedOn = dbUser.UpdatedOn,
                            LastLoginTime = dbUser.LastLoginTime,
                            IsDeleted = dbUser.IsDeleted
                        };
                        userDtoList.Add(userDto);
                    }
                }
                return userDtoList;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return null;
            }
        }

        public async Task<ActionMessageResponse> GetUserByOrgCodeAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    return new ActionMessageResponse { Success = false, Message = "Inputs cannot be Empty/Null", Content = "" };
                }
                List<UsersDto> userDtoList = new List<UsersDto>();
                var dbUser = await _usersRepository.GetUserByOrgCodeAsync(orgCode);
                if (dbUser == null)
                {
                    return new ActionMessageResponse { Success = false, Message = "User Not Found!!", Content = "" };
                }
                foreach (var user in dbUser)
                {
                    UsersDto userDto = new UsersDto()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        OrgCode = user.OrgCode,
                        Gender = user.Gender,
                        IsVerified = user.IsVerified,
                        ProfileAvatar = user.ProfileAvatar,
                        CreatedOn = user.CreatedOn,
                        UpdatedOn = user.UpdatedOn,
                        LastLoginTime = user.LastLoginTime,
                        IsDeleted = user.IsDeleted
                    };
                    userDtoList.Add(userDto);
                }
                return new ActionMessageResponse { Success = false, Message = "User  Found!!", Content = userDtoList };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse> UpsertUserProfileAsync( int userId, int profileId)
        {
            try
            {
                if (userId < 0 || profileId < 0)
                {
                    return new ActionMessageResponse { Success = false, Message = "Inputs cannot be Empty/Null", Content = "" };
                }
                var userData = await _usersRepository.GetUserByIdAsync(userId);
                if (userData == null)
                {
                    return new ActionMessageResponse { Success = false, Message = "UserId is not valid ", Content = "" };
                }
                var response = await _userProfilesService.UpsertUserProfileAsync(userId, profileId);
                return new ActionMessageResponse { Success = true, Message = "", Content = true };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }
    }
}
