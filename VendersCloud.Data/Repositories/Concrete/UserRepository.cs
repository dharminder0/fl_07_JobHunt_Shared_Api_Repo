using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserRepository : DataRepository<User>, IUserRepository
    {

        public async Task<IEnumerable<User>> GetAllUsersInfoAsync()
        {
            try
            {
                var sql = @"SELECT * FROM [USER]";
                return await QueryAsync<User>(sql);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging library)
                Console.WriteLine(ex.Message);
                throw; // Rethrow the original exception
            }
        }

        public async Task<User> UserLoginAsync(UserLoginRequestModel loginRequest)
        {
            try
            {
                var sql = @"SELECT * FROM [USER] WHERE EMAIL = @Email AND Password = @Password";

                var parameters = new
                {
                    Email = loginRequest.Email,
                    Password = loginRequest.Password
                };

                var result = await QueryAsync<User>(sql, parameters);

                if (result.Any())
                {
                    var sqlUpdate = @"UPDATE [USER] SET LASTLOGINTIME = GETUTCDATE() WHERE EMAIL = @Email AND Password = @Password";
                    await ExecuteAsync(sqlUpdate, parameters);

                    return result.FirstOrDefault();
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }
            }
            catch (Exception ex)
            {
                // Log the exception here (if logging is set up)
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<string> UpsertAsync(UserSignUpRequestModel usersign, string userId)
        {
            try
            {
                string CompanyName = usersign.CompanyName;
                string Password = usersign.Password;
                string Email = usersign.Email;

                var sql = @"
        IF EXISTS (SELECT 1 FROM [USER] WHERE EMAIL=@Email)
        BEGIN
            UPDATE [USER]
            SET
                FIRSTNAME = @CompanyName,
                PASSWORD = @Password,
                USERID = @userId
            WHERE EMAIL = @Email;
            SELECT UserId FROM [USER] WHERE EMAIL = @Email;
        END
        ELSE
        BEGIN
            INSERT INTO [USER] (FirstName, Email, Password, UserId)
            VALUES (@CompanyName, @Email, @Password, @userId);
            SELECT UserID FROM [USER] WHERE Email = @Email;
        END";

                var parameters = new { CompanyName, Password, Email, userId };

                // Execute the SQL command and capture the result
                var result = await QueryAsync<string>(sql, parameters);

                if (result.Any())
                {
                    return result.FirstOrDefault();
                }

                throw new Exception("Something went wrong during data insertion or update.");
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new ApplicationException("Error during upsert operation", ex);
            }
        }

        public async Task<bool> AddInformationAsync(CompanyInfoRequestModel companyInfo)
        {
            string userId = companyInfo.UserId;
            string companyName = companyInfo.CompanyName;
            string description = companyInfo.Portfolio;
            string mail = companyInfo.ContactMail;
            string phone = companyInfo.Phone;
            string website = companyInfo.Website;
            string strength = companyInfo.Strength;
            RoleType role = (RoleType)int.Parse(companyInfo.RegistrationType);
            string roleTypeString = role.ToString(); 

            string sql = @"
                         UPDATE [USER]
                         SET
                             Email = @Mail,
                             Phone = @Phone,
                             RoleType = @RoleType,
                             CreatedOn = GETDATE(),
                             UpdatedOn = GETDATE()
                          
                         WHERE UserId = @UserId";

            try
            {
                var rowsAffected = await ExecuteAsync(sql, new
                {
                     mail,
                     phone,
                    RoleType = roleTypeString,
                     userId
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }


    }
}
