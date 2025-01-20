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
                var result = await _userRepository.GetAllUsersInfoAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public async Task<ActionMessageResponseModel> UserLoginAsync(UserLoginRequestModel loginRequest)
        {
            try
            {
                if (!string.IsNullOrEmpty(loginRequest.Email) && !string.IsNullOrEmpty(loginRequest.Password))
                {
                    UserLoginResponseModel model = new UserLoginResponseModel();
                    var result = await _userRepository.UserLoginAsync(loginRequest);
                    model.UserId = result.UserId;
                    var mapping = await _userCompanyMappingRepository.GetMappingsByUserIdAsync(model.UserId);
                    var companyCode = mapping.CompanyCode;
                    var companydata = await _companyRepository.GetCompanyDetailByCompanyCodeAsync(companyCode);
                    model.Email = result.Email;
                    if (Enum.TryParse(result.RoleType, true, out RoleType role))
                    {
                        model.Role = ((int)role).ToString();
                    }
                    model.CompanyIcon = companydata.CompanyIcon;
                    model.CompanyName = companydata.CompanyName;
                    return new ActionMessageResponseModel() { Success = true,Message=" Login SuccessFull!",Content= model} ;

                }
                else
                {
                    return new ActionMessageResponseModel() { Success = false, Message = "The credentials can't be blank", Content = "" }; 
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g., log it
                return new ActionMessageResponseModel() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponseModel> UserSignUpAsync(UserSignUpRequestModel usersign)
        {
            try
            {
                if (string.IsNullOrEmpty(usersign.CompanyName) || (string.IsNullOrEmpty(usersign.Email)) || (string.IsNullOrEmpty(usersign.Password)))
                {
                    return new ActionMessageResponseModel { Success = false, Message = "Values can't be null", Content = "" };
                }
                //string userId = string.Empty;
                //string input = $"{email}-{DateTime.UtcNow}";
                //using (SHA256 sha256 = SHA256.Create())
                //{
                //    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                //    userId = $"USID"+BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
                //}
                string userId = usersign.Email;
                var rs = await _userRepository.UpsertAsync(usersign, userId);
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string companyCode = new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
                var res = await _companyRepository.UpsertAsync(usersign.CompanyName, usersign.Email, companyCode);
                await _userCompanyMappingRepository.AddMappingAsync(userId, companyCode);
                return new ActionMessageResponseModel { Success = true, Message = "UserId", Content = rs };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponseModel { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo)
        {
            try
            {
                if(!string.IsNullOrEmpty(companyInfo.UserId))
                {
                    var result= await _userRepository.AddInformationAsync(companyInfo);
                    return result;

                }
                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

