using Microsoft.Extensions.Configuration;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UsersRepository : StaticBaseRepository<Users>, IUsersRepository
    {
        public UsersRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();

                // Check if the organization exists in USERS table
                var query = new Query(tableName.TableName)
                    .Where("OrgCode", orgCode)
                    .Where("Username",request.Email)
                    .Select("Id")
                    .Select("OrgCode");

                var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

                if (!string.IsNullOrEmpty(existingOrgCode))
                {
                    // Update existing user record
                    var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            Password = hashedPassword,
                            PasswordSalt = salt,
                            Username= request.Email,
                            UpdateOn=DateTime.UtcNow,
                            LastLoginTime= DateTime.UtcNow
                        })
                        .Where("OrgCode", orgCode);

                    await dbInstance.ExecuteAsync(updateQuery);
                    return existingOrgCode;
                }

                // Insert new user
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    OrgCode = orgCode,
                    Password = hashedPassword,
                    PasswordSalt = salt,
                    Username = request.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    LastLoginTime = DateTime.UtcNow
                });

                var insertedUserId = await dbInstance.ExecuteScalarAsync<string>(insertQuery);

                // Re-fetch to ensure insertion
                var query2 = new Query(tableName.TableName).Where("Username",request.Email).Where("OrgCode", orgCode)
                    .Select("Id");

                var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(query2);

                return !string.IsNullOrEmpty(insertedOrgCode) ? insertedOrgCode : insertedUserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

    }

}
