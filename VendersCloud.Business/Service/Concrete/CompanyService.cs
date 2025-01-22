using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.DTOModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using VendersCloud.Data.Repositories.Concrete;

namespace VendersCloud.Business.Service.Concrete
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserCompanyMappingService _userCompanyMappingService;
        private readonly IUserService _userService;


        public CompanyService(ICompanyRepository companyRepository, IUserCompanyMappingService userCompanyMappingService, IUserService userService)
        {
            _companyRepository = companyRepository;
            _userCompanyMappingService = userCompanyMappingService;
            _userService = userService;
        }


        public async Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyCode))
                {

                    var result = await _companyRepository.GetCompanyDetailByCompanyCodeAsync(companyCode);
                    return result;
                }
                else
                {
                    throw new Exception("companyCode is empty/null");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> UpsertAsync(string companyName, string email, string companyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(companyCode))
                {
                    var result = await _companyRepository.UpsertAsync(companyName, email, companyCode);
                    return result;
                }
                else
                {
                    throw new Exception("Values can't be null/empty");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionMessageResponseModel> AddCompanyInformationAsync(CompanyInfoRequestModel companyInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(companyInfo.CompanyName) || string.IsNullOrEmpty(companyInfo.UserId))
                {
                    return new ActionMessageResponseModel() { Success = false, Message = "Values can't be null related to companyName/UserId", Content = "" };
                }
                var CompanyCode = string.Empty;
                var mappingData = await _userCompanyMappingService.GetMappingsByUserIdAsync(companyInfo.UserId);
                if (mappingData != null)
                {
                    CompanyCode = mappingData.CompanyCode;
                }
                var result = await _companyRepository.AddCompanyInformationAsync(companyInfo, CompanyCode);
                if (result)
                {
                    //var res = await _userService.AddInformationAsync(companyInfo);
                    //if (res)
                    //{
                        return new ActionMessageResponseModel() { Success = true, Message = "CompanyInformation is Added", Content = CompanyCode };
                   // }
                  //  return new ActionMessageResponseModel() { Success = false, Message = "While data is not found by userid", Content = "" };
                }
                return new ActionMessageResponseModel() { Success = false, Message = "", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponseModel() { Success = false, Message = ex.Message, Content = "" };
            }


        }

        public async Task<IEnumerable<Company>> GetAllCompanyDetailsAsync()
        {
            try
            {
                var result = await _companyRepository.GetAllCompanyDetailsAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<CompanyUserListDto>> GetCompanyUserListAsync(string companyCode)
        {
            try
            {
                CompanyUserListDto companyUserListDto = new CompanyUserListDto();
               
                companyUserListDto.Users = new List<UserDto>();  // Initialize the Users list
                
                var companyData = await _companyRepository.GetCompanyDetailByCompanyCodeAsync(companyCode);
                companyUserListDto.Id = companyData.Id;
                companyUserListDto.CompanyCode = companyCode;
                companyUserListDto.CompanyName = companyData.CompanyName;
                companyUserListDto.Phone = companyData.Phone;
                companyUserListDto.Email = companyData.Email;
                companyUserListDto.CreatedOn = companyData.CreatedOn;
                companyUserListDto.UpdatedOn = companyData.UpdatedOn;
                companyUserListDto.CompanyStrength = companyData.CompanyStrength;
                companyUserListDto.CompanyWebsite = companyData.CompanyWebsite;
                companyUserListDto.CompanyIcon = companyData.CompanyIcon;
                companyUserListDto.Description = companyData.Description;

                var mappingData = await _userCompanyMappingService.GetMappingsByCompanyCodeAsync(companyCode);
                foreach (var mapping in mappingData)
                {
                    var userId = mapping.UserId;
                    var userData = (await _userService.GetUserDetailsByUserIdAsync(userId)).FirstOrDefault();

                    if (userData != null)
                    {
                        UserDto userDto = new UserDto
                        {
                            Id = userData.Id,
                            Email = userData.Email,
                            Phone = userData.Phone,
                            FirstName = userData.FirstName,
                            LastName = userData.LastName,
                            CreatedOn = userData.CreatedOn,
                            UpdatedOn = userData.UpdatedOn,
                            LastLoginTime = userData.LastLoginTime,
                            UserId = userData.UserId
                        };

                        companyUserListDto.Users.Add(userDto);
                    }
                }

                return new List<CompanyUserListDto> { companyUserListDto };
            }
            catch (Exception ex)
            {
                // Handle the exception
                return null;
            }
        }


    }
}