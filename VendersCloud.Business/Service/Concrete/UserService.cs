using Microsoft.AspNetCore.Identity.Data;
using System.Data;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
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

        public async Task<IEnumerable<User>> GetAllUsersInfoAsync()
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
                var userLoginResponse = await _userRepository.UserLoginAsync(loginRequest);
                if (userLoginResponse == null)
                {
                    return new ActionMessageResponseModel { Success = false, Message = "Invalid credentials", Content = "" };
                }

                var companyData = await GetCompanyDataForUserAsync(userLoginResponse.UserId);

                var model = new UserLoginResponseModel
                {
                    UserId = userLoginResponse.UserId,
                    Email = userLoginResponse.Email,
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
                var loginRequest = new UserLoginRequestModel { Email = usersign.Email, Password = usersign.Password };
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
                var userCreationResult = await _userRepository.UpsertAsync(usersign, userId);
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
    }
}
