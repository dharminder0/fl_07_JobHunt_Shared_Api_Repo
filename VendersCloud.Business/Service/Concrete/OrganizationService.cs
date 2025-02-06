using Microsoft.AspNetCore.Identity.Data;
using System.Text;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using VendersCloud.Data.Repositories.Concrete;

namespace VendersCloud.Business.Service.Concrete
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserProfilesRepository _userProfilesRepository;
        private readonly IOrgProfilesRepository _orgProfilesService;
        private readonly IOrgLocationRepository _organizationLocationRepository;
        private readonly IOrgSocialRepository _organizationSocialRepository;
        public OrganizationService(IOrganizationRepository organizationRepository, IUserProfilesRepository userProfilesRepository, IOrgProfilesRepository _orgProfilesRepository, IOrgLocationRepository organizationLocationRepository, IOrgSocialRepository organizationSocialRepository)
        {
            _organizationRepository = organizationRepository;
            _userProfilesRepository = userProfilesRepository;
            _orgProfilesService = _orgProfilesRepository;
            _organizationLocationRepository = organizationLocationRepository;
            _organizationSocialRepository = organizationSocialRepository;
        }

        public async Task<string> RegisterNewOrganizationAsync(RegistrationRequest request)
        {
            try
            {
                string companyCode = GenerateRandomOrgCode();
                string orgcode = await _organizationRepository.RegisterNewOrganizationAsync(request, companyCode);
                return orgcode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GenerateRandomOrgCode()
        {
            Random _random = new Random();
            int length = 8;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[_random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        public async Task<Organization> GetOrganizationDataAsync(string orgCode)
        {
            try
            {
                var response = await _organizationRepository.GetOrganizationData(orgCode);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Organization>> GetOrganizationListAsync()
        {
            try
            {
                var response = await _organizationRepository.GetOrganizationListAsync();
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ActionMessageResponse> AddOrganizationInfoAsync(CompanyInfoRequest infoRequest)
        {
            try
            {
                if (infoRequest == null)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values can't be null", Content = "" };
                }
                int userId = Convert.ToInt32(infoRequest.UserId);

                var dbUser = await GetUserByIdAsync(userId);
                if (dbUser != null)
                {
                    bool response = await _organizationRepository.UpdateOrganizationByOrgCodeAsync(infoRequest, dbUser.OrgCode);
                    if (response)
                    {
                        foreach (var item in infoRequest.registrationType)
                        {
                            int pId = Convert.ToInt32(item);
                            var uPres = await _userProfilesRepository.InsertUserProfileAsync(userId, pId);
                            var oPres = await _orgProfilesService.AddOrganizationProfileAsync(dbUser.OrgCode, pId);

                        }
                        return new ActionMessageResponse() { Success = true, Message = "Organization Information is updated", Content = "" };

                    }
                    return new ActionMessageResponse() { Success = false, Message = "Organization Information is not updated", Content = "" };
                }
                return new ActionMessageResponse() { Success = false, Message = "Data not found!!", Content = "" };
            }
            catch (Exception ex)
            {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<UsersDto> GetUserByIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return null;
                }
                var dbUser = await _organizationRepository.GetUserByIdAsync(userId);
                if (dbUser == null)
                {
                    return null;
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
                return userdto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionMessageResponse> UpsertOrganizationProfile(OrganizationProfileRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter Valid Inputs", Content = "" };

                }
                var orgData = await GetOrganizationDataAsync(request.OrgCode);
                if (orgData == null)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Data against this OrgCode is not found!!", Content = "" };
                }
                request.OrgName = string.IsNullOrEmpty(request.OrgName) ? orgData.OrgName : request.OrgName;
                request.Phone = string.IsNullOrEmpty(request.Phone) ? orgData.Phone : request.Phone;
                request.Email = string.IsNullOrEmpty(request.Email) ? orgData.Email : request.Email;
                request.Website = string.IsNullOrEmpty(request.Website) ? orgData.Website : request.Website;
                request.EmpCount = request.EmpCount <= 0 ? orgData.EmpCount : request.EmpCount;
                request.Description = string.IsNullOrEmpty(request.Description) ? orgData.Description : request.Description;
                request.RegAddress = string.IsNullOrEmpty(request.RegAddress) ? orgData.RegAddress : request.RegAddress;

                CompanyInfoRequest infoRequest = new CompanyInfoRequest();
                infoRequest.OrgName = request.OrgName;
                infoRequest.ContactMail = request.Email;
                infoRequest.Portfolio = request.Description;
                infoRequest.Website = request.Website;
                infoRequest.Phone = request.Phone;
                infoRequest.Strength = request.EmpCount.ToString();
                await _organizationRepository.UpdateOrganizationByOrgCodeAsync(infoRequest, request.OrgCode);

                if (request.OfficeLocation != null)
                {
                    foreach (var orgLocation in request.OfficeLocation)
                    {
                        OrgLocation location = new OrgLocation();
                        location.City = orgLocation.City;
                        location.State = orgLocation.State;
                        location.OrgCode = request.OrgCode;

                        var res = await _organizationLocationRepository.UpsertLocation(location);
                    }

                }
                if (request.SocialLinks != null)
                {
                    foreach (var social in request.SocialLinks)
                    {
                        OrgSocial socials = new OrgSocial();
                        socials.Platform = social.Platform;
                        socials.Name = social.Name;
                        socials.URL = social.URL;
                        socials.OrgCode = request.OrgCode;
                        var response = await _organizationSocialRepository.UpsertSocialProfile(socials);
                    }
                }
                return new ActionMessageResponse() { Success = true, Message = "Data against this OrgCode is Update!!", Content = "" };

            }
            catch (Exception ex)
            {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }

        }

        public async Task<ActionMessageResponse> GetOrganizationProfile(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter Valid Input!!", Content = "" };
                }

                var response = await _organizationRepository.GetOrganizationData(orgCode);
                if (response != null)
                {
                    // Await the tasks to get actual data
                    var socialData = await _organizationSocialRepository.GetOrgSocialProfile(orgCode);
                    var orgLocationData = await _organizationLocationRepository.GetOrgLocation(orgCode);

                    // Initialize the profile response
                    OrganizationProfileResponse profileResponse = new OrganizationProfileResponse
                    {
                        OrgCode = orgCode,
                        OrgName = response.OrgName,
                        Phone = response.Phone,
                        Email = response.Email,
                        Website = response.Website,
                        EmpCount = response.EmpCount,
                        Logo = response.Logo,
                        Description = response.Description,
                        RegAddress = response.RegAddress,
                        IsDeleted = response.IsDeleted,
                        SocialLinks = new List<VendersCloud.Business.Entities.ResponseModels.SocialProfiles>(), // Use fully qualified name
                        OfficeLocation = new List<VendersCloud.Business.Entities.ResponseModels.OfficeLocations>() // Use fully qualified name
                    };

                    // Add social profiles to the list
                    foreach (var social in socialData)
                    {
                        profileResponse.SocialLinks.Add(new VendersCloud.Business.Entities.ResponseModels.SocialProfiles
                        {
                            Platform = social.Platform,
                            Name = social.Name,
                            URL = social.URL
                        });
                    }

                    // Add organization locations to the list
                    foreach (var dataLocation in orgLocationData)
                    {
                        profileResponse.OfficeLocation.Add(new VendersCloud.Business.Entities.ResponseModels.OfficeLocations
                        {
                            City = dataLocation.City,
                            State = dataLocation.State
                        });
                    }

                    return new ActionMessageResponse()
                    {
                        Success = true,
                        Message = "Organization Profile",
                        Content = profileResponse
                    };
                }

                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = "Enter Valid Input!!",
                    Content = ""
                };
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

    }
}
