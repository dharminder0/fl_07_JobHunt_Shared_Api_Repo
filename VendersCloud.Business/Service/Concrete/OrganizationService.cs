using System;
using System.Text;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        public OrganizationService(IOrganizationRepository organizationRepository) {
        _organizationRepository = organizationRepository;
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

        //public async Task<ActionMessageResponse> AddOrganizationInfo(CompanyInfoRequest infoRequest)
        //{
        //    try
        //    {
        //        if(infoRequest == null)
        //    }
        //    catch (Exception ex) { }
        //}
    }
}
