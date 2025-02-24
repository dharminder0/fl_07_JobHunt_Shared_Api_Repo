using Microsoft.Extensions.Configuration;
using System.Text;
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
        private readonly IUserProfilesService _userProfilesService;
        private IConfiguration _configuration;
        private CommunicationService _communicationService;
        private readonly IBlobStorageService _blobStorageService;
        public UsersService(IConfiguration configuration,IUsersRepository usersRepository, IOrganizationService organizationService, IUserProfilesService userProfilesService, IBlobStorageService blobStorageService)
        {
            _usersRepository = usersRepository;
            _organizationService = organizationService;
            _userProfilesService = userProfilesService;
            _configuration = configuration;
            _communicationService = new CommunicationService(configuration);
            _blobStorageService = blobStorageService;
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
                    var verificationOtp = GenerateOTP();
                    string token = Guid.NewGuid().ToString().ToLower();
                    var data = await _usersRepository.InsertUserAsync(request, hashedPassword, saltBytes, orgCode,verificationOtp,token);
                    if (data != null)
                    {
                        if(data.Equals("User Already Exists!!"))
                        {
                            return new ActionMessageResponse { Success = false, Message = "User Already Exists!!", Content = "" };
                        }
                        if (await _communicationService.SendUserVerificationEmail(request.FirstName, request.LastName, request.Email, verificationOtp, token))
                        {
                            RegistrationDto registration = new RegistrationDto();
                            registration.UserId = data;
                            registration.OrgCode = orgCode;
                            registration.Email = request.Email;
                            return new ActionMessageResponse { Success = true, Message = "New Client Registered Successfully!!", Content = registration };
                        }
                        return new ActionMessageResponse { Success = false, Message = "Enter Correct Email", Content = "" };
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
                    // string roleName = Enum.GetName(typeof(RoleType), userProfileRole);

                    List<string> userProfileRoles = userProfileRole.Select(role => role.ProfileId.ToString()).ToList();
                    if (hashedPassword== dbUser.Password)
                    {
                        LoginResponseDto login = new LoginResponseDto();
                        login.FirstName= dbUser.FirstName;
                        login.LastName= dbUser.LastName;
                        login.Phone = companyData.Phone;
                        login.Gender= dbUser.Gender;
                        login.UserId = dbUser.Id.ToString();
                        login.Email = dbUser.UserName;
                        login.OrgCode = dbUser.OrgCode;
                        login.Role = userProfileRoles;
                        login.CompanyIcon = companyData.Logo;
                        login.CompanyName = companyData.OrgName;
                        login.IsVerified = dbUser.IsVerified;
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
                    DOB = dbUser.DOB,
                    Phone= dbUser.Phone,
                    IsDeleted = dbUser.IsDeleted
                };
                return new ActionMessageResponse { Success = true, Message = "User  Found!!", Content = userdto };
            }
            catch (Exception ex) {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<UsersDto> GetUserByIdAsync(int userId)
        {
            try
            {
                if (userId<=0)
                {
                    return null;
                }
                var dbUser = await _usersRepository.GetUserByIdAsync(userId);
                if (dbUser == null)
                {
                    return  null;
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
                    DOB = dbUser.DOB,
                    Phone= dbUser.Phone,
                    IsDeleted = dbUser.IsDeleted
                };
                return userdto;
            }
            catch (Exception ex)
            {
                throw ex;
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
                            DOB = dbUser.DOB,
                            Phone= dbUser.Phone,
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
                        DOB = user.DOB,
                        Phone= user.Phone,
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

        public async Task<ActionMessageResponse> InsertUserProfileAsync( int userId, int profileId)
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
                var response = await _userProfilesService.InsertUserProfileAsync(userId, profileId);
                return new ActionMessageResponse { Success = true, Message = "", Content = true };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse> VerifyUserEmailAsync(string userToken, string Otp)
        {
            try
            {
                if(string.IsNullOrEmpty(userToken) && (string.IsNullOrEmpty(Otp)))
                    {
                    return new ActionMessageResponse { Success = false, Message = "Value's cannot be blank", Content = "" };
                }
                var response= await _usersRepository.VerifyUserEmailAsync(userToken, Otp);
                if (response)
                {
                    return new ActionMessageResponse { Success = true, Message = "User is Verified!!", Content = "" };
                }
                return new ActionMessageResponse { Success = false, Message = "Enter Valid otp!!", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse>ResendEmailVerificationAsync(string email)
        {
            try
            {
                var existingUser = await _usersRepository.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    string otp = GenerateOTP();
                    string token = Guid.NewGuid().ToString().ToLower();
                    var updateResponse=await _usersRepository.UpdateOtpAndTokenAsync(otp,token,email);
                    if(updateResponse)
                    {
                        if(await _communicationService.SendUserVerificationEmail(existingUser.FirstName, existingUser.LastName, existingUser.UserName, otp, token))
                        {
                            return new ActionMessageResponse { Success = true, Message = "Email is sent Successfully!!", Content = "" };
                        }

                        return new ActionMessageResponse { Success = false, Message = "Email Sending Fail !!", Content = "" };
                    }
                }
                return new ActionMessageResponse { Success = true, Message = "User is not registered!!", Content = "" };


            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }


        public async Task<ActionMessageResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            try
            {
                string uploadedimageUrl = string.Empty;
                if (request.ProfileAvatar != null && request.ProfileAvatar.Count > 0)
                {
                    List<string> uploadedLogos = new List<string>();
                    foreach (var file in request.ProfileAvatar)
                    {
                        uploadedimageUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);

                    }

                }
                var res = await _usersRepository.UpdateUserProfileAsync(request, uploadedimageUrl);
                if (res)
                {
                    return new ActionMessageResponse { Success = true, Message = "Profile is Updated!!", Content = "" };
                }
                return new ActionMessageResponse { Success = false, Message = "Profile is not Updated!!", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse> UpdateUserPasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || (string.IsNullOrEmpty(request.OldPassword)) || (string.IsNullOrEmpty(request.NewPassword)))
                {
                    return new ActionMessageResponse { Success = false, Message = "Enter Valid Inputs", Content = "" };
                }
                var dbUser = await _usersRepository.GetUserByEmailAsync(request.Email);
                if (dbUser != null && dbUser.PasswordSalt != null)
                {
                    var saltBytes = dbUser.PasswordSalt;
                    string salt = Convert.ToBase64String(saltBytes);
                    var hashedPassword = Hasher.HashPassword(salt, request.OldPassword);
                    if (hashedPassword == dbUser.Password)
                    {
                        string salts = Hasher.GenerateSalt();
                        byte[] saltBytess = Convert.FromBase64String(salts);
                        var hashedNewPassword = Hasher.HashPassword(salts, request.NewPassword);
                        var res= await _usersRepository.UpdateChangePasswordAsync(request,hashedNewPassword, saltBytess);
                        if (res)
                            return new ActionMessageResponse { Success = true, Message = "Password Changed Successfully!!", Content = "" };
                        return new ActionMessageResponse { Success = false, Message = "Password Not Changed!!", Content = "" };
                    }
                    return new ActionMessageResponse { Success = false, Message = "Password Not Matched!!", Content = "" };
                }
                return new ActionMessageResponse { Success = false, Message = "User Not Found!!", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }


        public async Task<ActionMessageResponse> AddOrganizationMemberAsync(AddMemberRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) || string.IsNullOrEmpty(request.Email)|| string.IsNullOrEmpty(request.OrgCode))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Enter Valid Inputs",
                        Content = ""
                    };
                }
                string salt = string.Empty;
                byte[] saltBytes = Convert.FromBase64String(salt);
                var verificationOtp = GenerateOTP();
                string token = Guid.NewGuid().ToString().ToLower();
                var companyData = await _organizationService.GetOrganizationDataAsync(request.OrgCode);
                RegistrationRequest registration = new RegistrationRequest();
                registration.Email = request.Email;
                registration.FirstName = request.FirstName;
                registration.LastName = request.LastName;
                registration.CompanyName = companyData.OrgName;
                var data = await _usersRepository.InsertUserAsync(registration, string.Empty, saltBytes, request.OrgCode, verificationOtp, token);
                if (data != null)
                {
                    var dbUser = await _usersRepository.GetUserByEmailAsync(request.Email);
                    var res = await _userProfilesService.InsertUserProfileAsync(dbUser.Id, request.Access);
                    if (data.Equals("User Already Exists!!"))
                    {
                        return new ActionMessageResponse { Success = false, Message = "Member Already Exists!!", Content = "" };
                    }
                    if (await _communicationService.SendUserVerificationEmail(request.FirstName, request.LastName, request.Email, verificationOtp, token))
                    {
                        return new ActionMessageResponse { Success = true, Message = "New Member Added Successfully!!", Content = "" };
                    }
                }
                return new ActionMessageResponse { Success = false, Message = "Not Added", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Content = ""
                };
            }
        }
        public static string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000,999999).ToString();
        }
    }
}
