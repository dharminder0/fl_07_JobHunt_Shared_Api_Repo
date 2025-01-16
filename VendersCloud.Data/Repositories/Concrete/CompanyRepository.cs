using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class CompanyRepository : DataRepository<Company>, ICompanyRepository
    {
        public async Task<Company> GetCompanyDetailByCompanyCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                throw new ArgumentException("Company code can't be blank");
            }

            var sql = "SELECT * FROM COMPANY WHERE COMPANYCODE = @companyCode";

            try
            {
                var result = await QueryAsync<Company>(sql, new { companyCode });
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving company details", ex);
            }
        }


        public async Task<string> Upsert(string companyName, string email, string companyCode)
        {
            try
            {
                var sql = @"
            IF EXISTS (SELECT 1 FROM [Company] WHERE EMAIL=@Email)
            BEGIN
                UPDATE [Company]
                SET
                    COMPANYNAME=@CompanyName
                WHERE EMAIL=@Email;
                SELECT CompanyCode FROM [Company] WHERE EMAIL=@Email;
            END
            ELSE
            BEGIN
                INSERT INTO [Company] (CompanyName, Email, CompanyCode)
                VALUES (@CompanyName, @Email, @companyCode);
                SELECT CompanyCode FROM [Company] WHERE EMAIL=@Email;
            END";

                var parameters = new { companyName, email, companyCode };

                var result = await QueryAsync<string>(sql, parameters);

                if (result.Any())
                {
                    return result.FirstOrDefault();
                }

                throw new Exception("Something went wrong during data insertion or update.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error during upsert operation", ex);
            }
        }

    }
}
