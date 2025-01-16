using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserCompanyMappingRepository : DataRepository<UserCompanyMapping>, IUserCompanyMappingRepository
    {
        public async Task<UserCompanyMapping> GetMappingsByUserId(string userId)
        {
            try
            {
                var sql = @"SELECT * FROM UserCompanyMapping WHERE USERID = @UserId";
                var parameters = new { UserId = userId };
                var result = await QueryAsync<UserCompanyMapping>(sql, parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task AddMapping(string userId, string companyCode)
        {
            try
            {
                var sql = @"INSERT INTO UserCompanyMapping (UserId, CompanyCode) VALUES (@userId, @companyCode)";
                await ExecuteAsync(sql, new { userId, companyCode });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error during mapping insertion", ex);
            }
        }

    }
}
