using System.Data;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.DTOModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using VendersCloud.Data.Repositories.Concrete;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserCompanyMappingService _userCompanyMappingService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;


        public CompanyService(ICompanyRepository companyRepository, IUserCompanyMappingService userCompanyMappingService, IUserService userService, IUserRepository userRepository)
        {
            _companyRepository = companyRepository;
            _userCompanyMappingService = userCompanyMappingService;
            _userService = userService;
            _userRepository = userRepository;
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
                    var res = await _userRepository.UpdateRoleAsync(companyInfo.UserId, companyInfo.RegistrationType);
                    if (res)
                    {
                        return new ActionMessageResponseModel() { Success = true, Message = "CompanyInformation is Added", Content = CompanyCode };
                    }
                    return new ActionMessageResponseModel() { Success = false, Message = "While data is not found by userid", Content = "" };
                }
                return new ActionMessageResponseModel() { Success = false, Message = "", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponseModel() { Success = false, Message = ex.Message, Content = "" };
            }


        }

        public async Task<ActionMessageResponseModel> GetAllCompanyDetailsAsync(string companyCode, List<string> roleType)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode) && (roleType == null || !roleType.Any()))
                {
                    var result = await _companyRepository.GetAllCompanyDetailsAsync();
                    return new ActionMessageResponseModel() { Success = true, Message = "List Of All Companies", Content = result };
                }
               else if (!string.IsNullOrEmpty(companyCode) && (roleType == null || !roleType.Any()))
                {
                    var res = await GetCompanyUserListAsync(companyCode);
                    return new ActionMessageResponseModel() { Success = true, Message = "List Of All User's With Company", Content = res };
                }
                else if(!string.IsNullOrEmpty(companyCode) && (roleType != null || roleType.Any()))
                {
                    // Flatten and remove duplicates
                    var roles = roleType.SelectMany(rt => rt.Split(',')).Distinct().ToList();

                    List<dynamic> allUsers = new List<dynamic>();

                    foreach (var role in roles)
                    {
                        var roleUsers = await GetCompanyUserListByRoleTypeAsync(companyCode, role);
                        if (roleUsers.Any() && roleUsers.Any() && roleUsers.Any(ru => ru.Users.Count > 0))
                        {
                            allUsers.AddRange(roleUsers);
                        }
                    }

                    return new ActionMessageResponseModel() { Success = true, Message = "List Of All User's With Company", Content = allUsers };
                }
                else if (string.IsNullOrEmpty(companyCode) && (roleType != null || roleType.Any()))
                {
                    var roles = roleType.SelectMany(rt => rt.Split(',')).Distinct().ToList();
                    List<dynamic> UserRecords = new List<dynamic>();
                    foreach (var role in roles)
                    {
                        var records = await _userRepository.GetUserDetailsByRoleAsync(role);
                        if(records.Any() && records.Any())
                        {
                            UserRecords.AddRange(records);
                        }

                    }
                    return new ActionMessageResponseModel() {Success=true,Message="Records by RoleType",Content= UserRecords };
                }
                return new ActionMessageResponseModel() { Success = false, Message = "", Content = "" };
            }
            catch (Exception ex)
            {
                // Use the built-in message instead of throwing the caught exception directly
                throw new Exception("An error occurred while fetching company details", ex);
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
                    var usersData = (await _userService.GetUserDetailsByUserIdAsync(userId));

                    if (usersData != null)
                    {
                        foreach (var userData in usersData)
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
                }

                return new List<CompanyUserListDto> { companyUserListDto };
            }
            catch (Exception ex)
            {
                // Handle the exception
                return null;
            }
        }

        public async Task<List<CompanyUserListDto>> GetCompanyUserListByRoleTypeAsync(string companyCode,string roleType)
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
                    var usersData = (await _userService.GetUserDetailsByRoleTypeAsync(userId, roleType));

                    if (usersData != null)
                    {
                        foreach (var userData in usersData)
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
                }

                return new List<CompanyUserListDto> { companyUserListDto };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}