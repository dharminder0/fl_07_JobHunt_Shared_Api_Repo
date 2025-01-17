using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
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
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
