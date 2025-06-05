using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrganizationService
    {
        Task<string> RegisterNewOrganizationAsync(RegistrationRequest request);
        Task<Organization> GetOrganizationDataAsync(string orgCode);
        Task<List<Organization>> GetOrganizationListAsync();
        Task<ActionMessageResponse> AddOrganizationInfoAsync(CompanyInfoRequest infoRequest);
        Task<ActionMessageResponse> UpsertOrganizationProfile(OrganizationProfileRequest request);
        Task<ActionMessageResponse> GetOrganizationProfile(GetProfileRequest request);
        Task<ActionMessageResponse> DispatchedOrganizationInvitationAsync(DispatchedInvitationRequest request);
        Task<bool> ManageRelationshipStatusAsync(ManageRelationshipStatusRequest request);
        Task<PaginationDto<OrgRelationshipSearchResponse>> GetListRelationshipAsync(OrgRelationshipSearchRequest request);
        Task<List<Notifications>> GetNotificationsAsync(string orgCode);
        Task<bool> UpsertNotificationAsync(int notificationId, bool isRead);
    }
}
