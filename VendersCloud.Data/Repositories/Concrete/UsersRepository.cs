using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
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
                    .Where("Username", request.Email)
                    .Where("IsDeleted", false)
                    .Select("Id")
                    .Select("OrgCode");

                var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

                if (!string.IsNullOrEmpty(existingOrgCode))
                {
                    string res = "User Already Exists!!";
                    return res;
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
                    LastLoginTime = DateTime.UtcNow,
                    IsDeleted = false
                });

                var insertedUserId = await dbInstance.ExecuteScalarAsync<string>(insertQuery);

                // Re-fetch to ensure insertion
                var query2 = new Query(tableName.TableName).Where("Username", request.Email).Where("OrgCode", orgCode)
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

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.UserName,Operator.Eq,email),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Log the exception (optional)
                return null;
            }
        }

        public async Task<bool> DeleteUserByEmailAndOrgCodeAsync(string email, string organizationCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();
                var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            IsDeleted = true,
                            UpdatedOn = DateTime.UtcNow,
                            LastLoginTime = DateTime.UtcNow
                        })
                        .Where("UserName", email)
                        .Where("OrgCode", organizationCode);

                await dbInstance.ExecuteAsync(updateQuery);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Users>> GetAllUserAsync()
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users";

                var users = dbInstance.Select<Users>(sql).ToList();
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Log the exception (optional)
                return null;
            }
        }

        public async Task<List<Users>> GetUserByOrgCodeAsync(string orgCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users Where OrgCode=@orgCode";

                var users = dbInstance.Select<Users>(sql, new {orgCode}).ToList();
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Log the exception (optional)
                return null;
            }

        }

        public async Task<Users> GetUserByIdAsync(int Id)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.Id,Operator.Eq,Id),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Log the exception (optional)
                return null;
            }
        }
    }

}
