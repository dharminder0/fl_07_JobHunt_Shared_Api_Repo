using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUsersService
    {
        Task<ActionMessageResponse> RegisterNewUserAsync(RegistrationRequest request);
        Task<ActionMessageResponse> LoginUserAsync(LoginRequest request);
        Task<ActionMessageResponse> DeleteUserAsync(string email, string organizationCode);
        Task<ActionMessageResponse> GetUserByEmailAsync(string email);
        Task<List<UsersDto>> GetAllUserAsync();
        Task<ActionMessageResponse> GetUserByOrgCodeAsync(string orgCode);
        Task<ActionMessageResponse> InsertUserProfileAsync(int userId, int profileId);
        Task<UsersDto> GetUserByIdAsync(int userId);
        Task<ActionMessageResponse> VerifyUserEmailAsync(string userToken, string Otp);
        Task<ActionMessageResponse> ResendEmailVerificationAsync(string email);
        Task<ActionMessageResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task<ActionMessageResponse> UpdateUserPasswordAsync(ChangePasswordRequest request);
        Task<ActionMessageResponse> AddOrganizationMemberAsync(AddMemberRequest request);
        Task<ActionMessageResponse> SetPasswordAsync(SetPasswordRequest request);
        Task<PaginationDto<UsersDto>> SearchMemberDetailsAsync(SearchMemberRequest request);
        Task<ActionMessageResponse> ChangeEmailAsync(string oldEmail, string newEmail);
        Task<bool> DeleteMemberByIdAsync(int userId);
    }
}
