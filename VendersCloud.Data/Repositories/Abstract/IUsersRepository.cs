namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUsersRepository : IBaseRepository<Users>
    {
        Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode, string verificationOtp, string token, string phone);
        Task<Users> GetUserByEmailAsync(string email);
        Task<bool> DeleteUserByEmailAndOrgCodeAsync(string email, string organizationCode);
        Task<List<Users>> GetAllUserAsync();
        Task<List<Users>> GetUserByOrgCodeAsync(string orgCode);
        Task<Users> GetUserByIdAsync(int Id);
        Task<bool> VerifyUserEmailAsync(string userToken, string Otp);
        Task<bool> UpdateOtpAndTokenAsync(string otp, string token, string email);
        Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest request,string uploadedimageUrl);
        Task<bool> UpdateChangePasswordAsync(ChangePasswordRequest request, string hashedPassword, byte[] salt);
        Task<Users> GetUserByEmailAndOrgCodeAsync(string email, string orgCode);
        Task<Users> GetUserByUserTokenAsync(string userToken);
        Task<bool> SetUserPasswordAsync(string hashedPassword, byte[] salt, string userToken);
        Task<PaginationDto<UsersDto>> SearchMemberDetailsAsync(SearchMemberRequest request);
        Task<bool> UpdateEmailAsync(string OldEmail, string NewEmail, string verificationOtp);
    }
}
