using DapperExtensions.Predicate;
using DapperExtensions;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;
using Dapper;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class CompanyRepository : DataRepository<Company>, ICompanyRepository
    {
        public async Task<Company> GetCompanyDetailByCompanyCodeAsync(string companyCode)
        {
           
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    throw new ArgumentException("Company code can't be blank");
                }

                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<Company>(ucm => ucm.CompanyCode, Operator.Eq, companyCode));
                var result = await GetListByAsync(pg);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving company details", ex);
            }
        }


        public async Task<string> UpsertAsync(string companyName, string email, string companyCode)
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

        public async Task<bool> AddCompanyInformationAsync(CompanyInfoRequestModel companyInfo, string companyCode)
        {
            try
            {
                var UserId = companyInfo.UserId;
                var CompanyName = companyInfo.CompanyName;
                var Description = companyInfo.Portfolio;
                var Mail = companyInfo.ContactMail;
                var Phone = companyInfo.Phone;
                var Website = companyInfo.Website;
                var Strength = companyInfo.Strength;

                var sql = @"Update [Company] 
                    SET
                    Phone = @Phone,
                    CreatedOn = GetDate(),
                    UpdatedOn = GetDate(),
                    CompanyWebsite = @Website,
                    CompanyStrength = @Strength,
                    Description = @Description
                    Where 
                    CompanyCode = @companyCode";

                var rowsAffected = await ExecuteAsync(sql, new
                {
                    Phone,
                    Website,
                    Strength,
                    Description,
                    companyCode
                });

                // Return true if one or more rows were affected
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, rethrow it, return a default value, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Company>> GetAllCompanyDetailsAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    var company = await connection.QueryAsync<Company>("SELECT * FROM [Company]");
                    return company.ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
