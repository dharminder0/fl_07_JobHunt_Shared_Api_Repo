﻿using System.Data;
using Microsoft.AspNetCore.SignalR;
using VendersCloud.Business.CommonMethods;

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
        private readonly IPartnerVendorRelRepository _partnerVendorRelRepository;   
        private readonly IOrgRelationshipsRepository _organizationRelationshipsRepository;
        private readonly CommunicationService _communicationService;
        private readonly IBlobStorageService _blobStorageService;
        private IConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrganizationService(IConfiguration configuration,IOrganizationRepository organizationRepository, IUserProfilesRepository userProfilesRepository,
            IOrgProfilesRepository _orgProfilesRepository, IOrgLocationRepository organizationLocationRepository, IOrgSocialRepository organizationSocialRepository, 
            IListValuesRepository listValuesRepository,IUsersRepository usersRepository, IOrgRelationshipsRepository organizationRelationshipsRepository,
            IBlobStorageService blobStorageService, IPartnerVendorRelRepository partnerVendorRelRepository, INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
        {
            _organizationRepository = organizationRepository;
            _userProfilesRepository = userProfilesRepository;
            _orgProfilesService = _orgProfilesRepository;
            _organizationLocationRepository = organizationLocationRepository;
            _organizationSocialRepository = organizationSocialRepository;
            _listValuesRepository = listValuesRepository;
            _usersRepository = usersRepository;
            _configuration = configuration;
            _partnerVendorRelRepository = partnerVendorRelRepository;   
            _organizationRelationshipsRepository =organizationRelationshipsRepository;
            _communicationService = new CommunicationService(usersRepository,configuration);
            _blobStorageService = blobStorageService;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task<string> RegisterNewOrganizationAsync(RegistrationRequest request)
        {
            try
            {
                string companyCode = CommonFunctions.GenerateRandomOrgCode();
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
                if (request.Logo != null && request.Logo.Count > 0 )
                {
                    List<string> uploadedLogos = new List<string>();
                    foreach (var file in request.Logo)
                    {
                        if(string.IsNullOrEmpty(file.FileData) )
                        {
                            continue;   
                        }
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

        public async Task<ActionMessageResponse> GetOrganizationProfile(GetProfileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OrgCode))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Enter Valid Input!!", Content = "" };
                }

                var response = await _organizationRepository.GetOrganizationData(request.OrgCode);
                if (response != null)
                {
                    // Await the tasks to get actual data
                    var socialData = await _organizationSocialRepository.GetOrgSocialProfile(request.OrgCode);
                    var orgLocationData = await _organizationLocationRepository.GetOrgLocation(request.OrgCode);

                    // Initialize the profile response
                    OrganizationProfileResponse profileResponse = new OrganizationProfileResponse
                    {
                        OrgCode = request.OrgCode,
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
                        OfficeLocation = new List<VendersCloud.Business.Entities.ResponseModels.OfficeLocations>(), // Use fully qualified name
                        Id=response.Id, 
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

                    var orgRelationshipData = await _partnerVendorRelRepository.ManagePartnerStatusAsync(request.RelatedOrgCode, request.OrgCode);
                    var orgRelationdata= orgRelationshipData;
                    if (orgRelationdata != null)
                    {
                        profileResponse.Status = orgRelationdata.StatusId; 
                        profileResponse.StatusName = CommonFunctions.GetEnumDescription((InviteStatus)orgRelationdata.StatusId);
                    }
                    else
                    {
                        profileResponse.Status = 0;
                        profileResponse.StatusName = "Not Invited";
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

        public async Task<ActionMessageResponse> DispatchedOrganizationInvitationAsync(DispatchedInvitationRequest request)
        {
            if (string.IsNullOrEmpty(request.PartnerCode) ||
                string.IsNullOrEmpty(request.VendorCode) ||
                request.CreatedBy <= 0)
            {
                return new ActionMessageResponse
                {
                    Content = null,
                    Message = "Invalid request payload!",
                    Success = false
                };
            }

            var newRelation = new PartnerVendorRel
            {
                PartnerCode = request.PartnerCode,
                VendorCode = request.VendorCode,
                StatusId = request.StatusId,
                CreatedBy = request.CreatedBy,
                UpdatedBy = request.UpdatedBy,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
               Comment = request.Message   ,   
                IsDeleted = false
            };

            var insertResult = await _partnerVendorRelRepository.UpsertPartnerVendorRelAsync(newRelation);
            if (insertResult == null)
            {
                return new ActionMessageResponse
                {
                    Content = null,
                    Message = "Failed to create relationship",
                    Success = false
                };
            }

            var vendorObj = await _organizationRepository.GetOrganizationData(request.VendorCode);
            var partnerObj = await _organizationRepository.GetOrganizationData(request.PartnerCode);

            if (vendorObj == null || partnerObj == null)
            {
                return new ActionMessageResponse
                {
                    Content = null,
                    Message = "Failed to retrieve organization data",
                    Success = false
                };
            }

            bool emailSent = await _communicationService.DispatchedInvitationMailAsync(
      vendorObj.OrgName,
      partnerObj.OrgName,
      partnerObj.Email ?? string.Empty,
      vendorObj.Email ?? string.Empty,
      request.Message);

            string message = emailSent
                ? "Dispatched Invitation successfully and email sent."
                : "Dispatched Invitation successfully but failed to send email.";
            try
            {
              
                string notificationMessage = $"Invitation sent to {vendorObj.OrgName} by {partnerObj.OrgName} with message: {request.Message}";
                string title = $" Invitation Recieved  from  {partnerObj.OrgName} ";   

                await _notificationRepository.InsertNotificationAsync(
                    vendorObj.OrgCode,
                    notificationMessage,
                    (int)NotificationType.VendorEmpanelled,
                    title
                );

               
                await _hubContext.Clients.Group(partnerObj.OrgCode)
                    .SendAsync("ReceiveNotification", new
                    {
                        OrgCode = vendorObj.OrgCode,
                        Message = notificationMessage,
                        NotificationType = (int)NotificationType.VendorEmpanelled,
                        title,
                        CreatedOn = DateTime.UtcNow
                    });
            }
            catch (Exception)
            {

                throw;
            }

            return new ActionMessageResponse
            {
                Content = insertResult,
                Message = message,
                Success = true
            };
        }




        public async Task<bool> ManageRelationshipStatusAsync(ManageRelationshipStatusRequest request)
        {
            try
            {
                if (request.PartnerVendorRelId <= 0 || request.StatusId <= 0)
                {
                    throw new ArgumentException("Provide Valid Input!!");
                }

                var result = await _partnerVendorRelRepository.ManagePartnerStatusAsync(request);

                if (result)
                {
         
                    var relationInfo = await _partnerVendorRelRepository.GetByIdAsync(request.PartnerVendorRelId);

                    if (relationInfo != null)
                    {
                        var partnerObj = relationInfo.PartnerCode; 
                        var vendorObj = relationInfo.VendorCode;
                        var vendorInfo =await  _organizationRepository.GetOrganizationData(vendorObj);

                        string notificationMessage = $"Your empanelment request has been accepted by {vendorInfo.OrgName}";
                        string title = $"Empanelment Accepted by {vendorInfo.OrgName}";

                        await _notificationRepository.InsertNotificationAsync(
                            partnerObj,
                            notificationMessage,
                            (int)NotificationType.VendorEmpanelled,
                            title
                        );

                        await _hubContext.Clients.Group(partnerObj)
                            .SendAsync("ReceiveNotification", new
                            {
                                OrgCode = partnerObj,
                                Message = notificationMessage,
                                NotificationType = (int)NotificationType.EmpanelledStatus,
                                Title = title,
                                CreatedOn = DateTime.UtcNow
                            });
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // It's better to throw the original exception without wrapping
                throw;
            }
        }


        public async Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request)
        {
            try
            {
                var response = new List<OrgRelationshipSearchResponse>();
                OrgRelationshipSearchResponse searchResponse = new OrgRelationshipSearchResponse();
                return await _partnerVendorRelRepository.GetListRelationshipAsync(request);

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task< List<Notifications>> GetNotificationsAsync(NotificationsRequest obj)
        {
            return await _notificationRepository.GetNotificationsAsync(obj);    
        }
        public async Task<bool> UpsertNotificationAsync(int notificationId, bool isRead)
        {
            var result= await _notificationRepository.UpsertNotificationAsync(notificationId,isRead);
            if (result)
            {
            
                await _hubContext.Clients.All.SendAsync("ReceiveNotificationUpdate", new
                {
                    NotificationId = notificationId,
                    IsRead = isRead
                });
            }

            return result;
        }
    

        public async Task<int> GetNotificationsCountAsync(string  orgCode)
        {
            int updatedCount = await _notificationRepository.GetNotificationsCountAsync(orgCode);

            await _hubContext.Clients.Group(orgCode).SendAsync("ReceiveNotificationCountUpdate", new
            {
                OrgCode = orgCode,
                Count = updatedCount
            });
            return updatedCount;
        }
    }
}
