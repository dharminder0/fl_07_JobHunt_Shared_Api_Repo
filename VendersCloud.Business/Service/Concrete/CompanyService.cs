using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserCompanyMappingService _userCompanyMappingService;
        private readonly IUserService _userService;

        public CompanyService(ICompanyRepository companyRepository, IUserCompanyMappingService userCompanyMappingService,IUserService userService)
        {
            _companyRepository = companyRepository;
            _userCompanyMappingService = userCompanyMappingService;
            _userService = userService;
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
                    var res = await _userService.AddInformationAsync(companyInfo);
                    if (res)
                    {
                        return new ActionMessageResponseModel() { Success = true, Message = "CompanyInformation is Added", Content = "" };
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
    }
}
