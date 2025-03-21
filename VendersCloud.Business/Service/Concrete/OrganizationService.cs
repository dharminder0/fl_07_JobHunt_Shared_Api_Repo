using System.Data;
using System.Text;
using VendersCloud.Business.Common_Methods;

namespace VendersCloud.Business.Service.Concrete
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserProfilesRepository _userProfilesRepository;
        private readonly IOrgProfilesRepository _orgProfilesService;
        private readonly IOrgLocationRepository _organizationLocationRepository;
        private readonly IOrgSocialRepository _organizationSocialRepository;
        private readonly IListValuesRepository _listValuesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IOrgRelationshipsRepository _organizationRelationshipsRepository;
        private readonly CommunicationService _communicationService;
        private readonly IBlobStorageService _blobStorageService;
        private IConfiguration _configuration;
        public OrganizationService(IConfiguration configuration,IOrganizationRepository organizationRepository, IUserProfilesRepository userProfilesRepository, IOrgProfilesRepository _orgProfilesRepository, IOrgLocationRepository organizationLocationRepository, IOrgSocialRepository organizationSocialRepository, IListValuesRepository listValuesRepository,IUsersRepository usersRepository, IOrgRelationshipsRepository organizationRelationshipsRepository,IBlobStorageService blobStorageService)
        {
            _organizationRepository = organizationRepository;
            _userProfilesRepository = userProfilesRepository;
            _orgProfilesService = _orgProfilesRepository;
            _organizationLocationRepository = organizationLocationRepository;
            _organizationSocialRepository = organizationSocialRepository;
            _listValuesRepository = listValuesRepository;
            _usersRepository = usersRepository;
            _configuration = configuration;
            _organizationRelationshipsRepository =organizationRelationshipsRepository;
            _communicationService = new CommunicationService(configuration);
            _blobStorageService = blobStorageService;
        }

        public async Task<string> RegisterNewOrganizationAsync(RegistrationRequest request)
        {
            try
            {
                string companyCode = CommonMethods.GenerateRandomOrgCode();
                string orgcode = await _organizationRepository.RegisterNewOrganizationAsync(request, companyCode);
                return orgcode;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                    bool response = await _organizationRepository.UpdateOrganizationByOrgCodeAsync(infoRequest, dbUser.OrgCode,string.Empty);
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
                    CreatedOn = dbUser.CreatedOn.ToString("dd-MM-yyyy"),
                    UpdatedOn = dbUser.UpdatedOn.ToString("dd-MM-yyyy"),
                    LastLoginTime = dbUser.LastLoginTime.ToString("dd-MM-yyyy"),
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
                var uploadedimageUrl = string.Empty;
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
                if (request.Logo != null && request.Logo.Count > 0)
                {
                    List<string> uploadedLogos = new List<string>();
                    foreach (var file in request.Logo)
                    {
                        uploadedimageUrl = await _blobStorageService.UploadBase64ToBlobAsync(file);

                    }

                }
                CompanyInfoRequest infoRequest = new CompanyInfoRequest();
                infoRequest.OrgName = request.OrgName;
                infoRequest.ContactMail = request.Email;
                infoRequest.Portfolio = request.Description;
                infoRequest.Website = request.Website;
                infoRequest.Phone = request.Phone;
                infoRequest.Strength = request.EmpCount.ToString();
                await _organizationRepository.UpdateOrganizationByOrgCodeAsync(infoRequest, request.OrgCode, uploadedimageUrl);
                if(!string.IsNullOrEmpty(request.RegAddress))
                {
                    await _organizationRepository.UpdateOrganizationAddressByOrgCodeAsync(request.RegAddress, request.OrgCode);
                }

                if (request.OfficeLocation != null)
                {
                    await _organizationLocationRepository.DeleteOrgLocationAsync(request.OrgCode);
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
                    await _organizationSocialRepository.DeleteOrgSocialAsync(request.OrgCode);
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
                        var data = await _listValuesRepository.GetListValuesAsync();
                        var selectedValues = data.Where(x => x.Id == dataLocation.State).Select(x => x.Value).FirstOrDefault();

                        profileResponse.OfficeLocation.Add(new VendersCloud.Business.Entities.ResponseModels.OfficeLocations
                        {
                            City = dataLocation.City,
                            State = dataLocation.State,
                            StateName = selectedValues
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

        public async Task<bool> DispatchedOrganizationInvitationAsync(DispatchedInvitationRequest request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.Sender.Email)|| string.IsNullOrEmpty(request.Sender.OrgCode)|| string.IsNullOrEmpty(request.Receiver.Email)|| string.IsNullOrEmpty(request.Receiver.OrgCode)|| string.IsNullOrEmpty(request.Message))
                {
                    throw new ArgumentException("Enter Valid Inputs!!!");
                }
                int status = 0;
                var dbuser = await _usersRepository.GetUserByEmailAndOrgCodeAsync(request.Sender.Email, request.Sender.OrgCode);
                var relationshipType = Enum.GetName(typeof(RoleType), request.Sender.RoleType);
                var roleMapping = new Dictionary<string, string>
                    {
                        { "Vendor", "1" },
                        { "Client", "2" }
                    };
                if (roleMapping.ContainsKey(relationshipType))
                {
                    relationshipType = roleMapping[relationshipType];
                }
                //Checking OrgCode is Vendor/Client
                var orgProfiles = await _orgProfilesService.GetOrgProfilesByOrgCodeAsync(request.Receiver.OrgCode);
                var userProfiles = await _userProfilesRepository.GetProfileRole(dbuser.Id);
                var selectedOrgProfile = orgProfiles.Where(x => x.ProfileId != request.Sender.RoleType).ToList();
                if (selectedOrgProfile == null)
                {
                    throw new ArgumentException("Organization as per your request is not found !!");
                }
                var selectedUserProfile= userProfiles.Where(x=>x.ProfileId==(request.Sender.RoleType)).ToList();
                var dbOrgReciver = await _organizationRepository.GetOrganizationByEmailAndOrgCodeAsync(request.Receiver.Email, request.Receiver.OrgCode);
                var dbOrgSender = await _organizationRepository.GetOrganizationData(request.Sender.OrgCode);
                if (await _communicationService.DispatchedInvitationMailAsync(dbOrgReciver.OrgName, dbOrgSender.OrgName,request.Sender.Email, request.Receiver.Email,request.Message))
                {
                    status = 1;
                }
                    var res = await _organizationRelationshipsRepository.AddOrgRelationshipDataAsync(request.Sender.OrgCode, request.Receiver.OrgCode, relationshipType, status, dbuser.Id);
                return true;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ManageRelationshipStatusAsync(int orgRelationshipId, int status)
        {
            try
            {
                if(orgRelationshipId<=0||status<=0)
                {
                    throw new ArgumentException("Provide Valid Input!!");
                }

                var result= await _organizationRelationshipsRepository.ManageRelationshipStatusAsync(orgRelationshipId, status);
                if (result)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request)
        {
            try
            {
                var response = new List<OrgRelationshipSearchResponse>();
                OrgRelationshipSearchResponse searchResponse = new OrgRelationshipSearchResponse();
                return await _organizationRelationshipsRepository.GetListRelationshipAsync(request);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
