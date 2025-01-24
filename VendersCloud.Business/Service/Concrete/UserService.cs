using Microsoft.AspNetCore.Identity.Data;
using System.Data;
using System.Globalization;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Common.Utils;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCompanyMappingRepository _userCompanyMappingRepository;
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository userRepository, IUserCompanyMappingRepository userCompanyMappingRepository, ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _userCompanyMappingRepository = userCompanyMappingRepository;
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<Users>> GetAllUsersInfoAsync()
        {
            try
            {
                return await _userRepository.GetAllUsersInfoAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if needed (can be extended with a logging service)
                throw new InvalidOperationException("Failed to retrieve user information.", ex);
            }
        }

        public async Task<ActionMessageResponseModel> UserLoginAsync(UserLoginRequestModel loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return new ActionMessageResponseModel { Success = false, Message = "The credentials can't be blank", Content = "" };
            }

            try
            {
                
                var userdata = await _userRepository.GetUserByEmail(loginRequest.Email);
                var password = userdata.Select(ru => ru.PasswordSalt).FirstOrDefault();
                var saltBytes = password;
                var hashedPassword = Hasher.HashPassword(saltBytes, loginRequest.Password);
                loginRequest.Password = hashedPassword;
                var userLoginResponse = await _userRepository.UserLoginAsync(loginRequest);
                if (userLoginResponse == null)
                {
                    return new ActionMessageResponseModel { Success = false, Message = "Invalid credentials", Content = "" };
                }

                var companyData = await GetCompanyDataForUserAsync(userLoginResponse.UserId);

                var model = new UserLoginResponseModel
                {
                    UserId = userLoginResponse.UserId,
                    Email = companyData.Email,
                    Role = Enum.TryParse(userLoginResponse.RoleType, true, out RoleType role) ? ((int)role).ToString() : null,
                    CompanyIcon = companyData.CompanyIcon,
                    CompanyName = companyData.CompanyName
                };

                return new ActionMessageResponseModel { Success = true, Message = "Login Successful!", Content = model };
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new ActionMessageResponseModel { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponseModel> UserSignUpAsync(UserSignUpRequestModel usersign)
        {
            if (string.IsNullOrEmpty(usersign.CompanyName) || string.IsNullOrEmpty(usersign.Email) || string.IsNullOrEmpty(usersign.Password))
            {
                return new ActionMessageResponseModel { Success = false, Message = "Values can't be null", Content = "" };
            }

            try
            {
                string userId = usersign.Email;
                var oldpass = usersign.Password;
                // Create the user
                var userCreationResult = await CreateUserAsync(usersign, userId);
                if (!userCreationResult.Success)
                    return userCreationResult;

                // Create the company and associate with the user
                string companyCode = GenerateCompanyCode();
                var companyCreationResult = await CreateCompanyAsync(usersign.CompanyName, usersign.Email, companyCode);
                if (!companyCreationResult.Success)
                    return companyCreationResult;

                // Map the user to the company
                await _userCompanyMappingRepository.AddMappingAsync(userId, companyCode);

                // Log the user in
                var loginRequest = new UserLoginRequestModel { Email = usersign.Email, Password = oldpass };
                return await UserLoginAsync(loginRequest); // Reuse login logic after signup
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new ActionMessageResponseModel { Success = false, Message = ex.Message, Content = "" };
            }
        }

        private async Task<ActionMessageResponseModel> CreateUserAsync(UserSignUpRequestModel usersign, string userId)
        {
            try
            {
                var salt = Hasher.GenerateSalt();
                var hashedPassword= Hasher.HashPassword(salt, usersign.Password);
                usersign.Password = hashedPassword;
                var userCreationResult = await _userRepository.UpsertAsync(usersign, userId,salt);
                if (userCreationResult == null)
                {
                    return new ActionMessageResponseModel { Success = false, Message = "User creation failed", Content = "" };
                }

                return new ActionMessageResponseModel { Success = true, Message = "User created successfully", Content = "" };
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new ActionMessageResponseModel { Success = false, Message = $"Error creating user: {ex.Message}", Content = "" };
            }
        }

        private async Task<ActionMessageResponseModel> CreateCompanyAsync(string companyName, string email, string companyCode)
        {
            try
            {
                var companyCreationResult = await _companyRepository.UpsertAsync(companyName, email, companyCode);
                if (companyCreationResult == null)
                {
                    return new ActionMessageResponseModel { Success = false, Message = "Company creation failed", Content = "" };
                }

                return new ActionMessageResponseModel { Success = true, Message = "Company created successfully", Content = "" };
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new ActionMessageResponseModel { Success = false, Message = $"Error creating company: {ex.Message}", Content = "" };
            }
        }

        private string GenerateCompanyCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task<Company> GetCompanyDataForUserAsync(string userId)
        {
            var mapping = await _userCompanyMappingRepository.GetMappingsByUserIdAsync(userId);
            if (mapping == null) throw new InvalidOperationException("User is not mapped to any company.");

            var companyData = await _companyRepository.GetCompanyDetailByCompanyCodeAsync(mapping.CompanyCode);
            if (companyData == null) throw new InvalidOperationException("Company not found.");

            return companyData;
        }

        public async Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo)
        {
            if (string.IsNullOrEmpty(companyInfo.UserId))
            {
                return false;
            }

            try
            {
                return await _userRepository.AddInformationAsync(companyInfo);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new InvalidOperationException("Failed to add company information.", ex);
            }
        }

        public async Task<IEnumerable<Users>> GetUserDetailsByUserIdAsync(string userId)
        {
            try
            {
                var result = await _userRepository.GetUserDetailsByUserIdAsync(userId);
                return result;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<IEnumerable<Users>> GetUserDetailsByRoleTypeAsync(string userId,string roletype)
        {
            try
            {
                int number;
                bool isInteger = int.TryParse(roletype, out number);

                string roleTypeString;
                if (isInteger)
                {
                    RoleType role = (RoleType)number;
                    roleTypeString = role.ToString();
                }
                else
                {
                    roleTypeString = roletype.ToString();
                }
                var result = await _userRepository.GetUserDetailsByRoleTypeAsync(userId, roleTypeString);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
