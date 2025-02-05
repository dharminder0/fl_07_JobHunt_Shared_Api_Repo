﻿using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUsersRepository : IBaseRepository<Users>
    {
        Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode, string verificationOtp, string token);
        Task<Users> GetUserByEmailAsync(string email);
        Task<bool> DeleteUserByEmailAndOrgCodeAsync(string email, string organizationCode);
        Task<List<Users>> GetAllUserAsync();
        Task<List<Users>> GetUserByOrgCodeAsync(string orgCode);
        Task<Users> GetUserByIdAsync(int Id);
        Task<bool> VerifyUserEmailAsync(string userToken, string Otp);
        Task<bool> UpdateOtpAndTokenAsync(string otp, string token, string email);
    }
}
