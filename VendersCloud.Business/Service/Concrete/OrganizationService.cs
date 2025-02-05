using AutoMapper;
using System;
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
        public OrganizationService(IOrganizationRepository organizationRepository, IUserProfilesRepository userProfilesRepository) {
        _organizationRepository = organizationRepository;
            _userProfilesRepository= userProfilesRepository;
        }

        public async Task<string> RegisterNewOrganizationAsync(RegistrationRequest request)
        {
            try
            {
               string companyCode = GenerateRandomOrgCode();
               string orgcode= await _organizationRepository.RegisterNewOrganizationAsync(request, companyCode);
                return orgcode;
            }
            catch (Exception ex) {
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
            catch(Exception ex)
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
                    if (response) { 
                        foreach(var item in infoRequest.registrationType)
                        {
                            int pId = Convert.ToInt32(item);
                            var res = await _userProfilesRepository.InsertUserProfileAsync(userId, pId);
                            
                        }
                       
                        return new ActionMessageResponse() { Success = true, Message = "Organization Information is updated", Content = "" };
                    }
                    return new ActionMessageResponse() { Success = false, Message = "Organization Information is not updated", Content = "" };
                }
                return new ActionMessageResponse() { Success = false, Message = "Data not found!!", Content = "" };
            }
            catch (Exception ex) {
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

        
    }
}
