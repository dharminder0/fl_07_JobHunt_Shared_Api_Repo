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
        public async Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode,string verificationOtp,string token)
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
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    OrgCode = orgCode,
                    Password = hashedPassword,
                    PasswordSalt = salt,
                    Username = request.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    LastLoginTime = DateTime.UtcNow,
                    IsDeleted = false,
                    VerificationToken = verificationOtp,
                    Token=token
                });

                var insertedUserId = await dbInstance.ExecuteScalarAsync<string>(insertQuery);

                // Re-fetch to ensure insertion
                var query2 = new Query(tableName.TableName).Where("Username", request.Email).Where("OrgCode", orgCode)
                    .Select("Id");

                var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(query2);

                return !string.IsNullOrEmpty(insertedOrgCode) ? insertedOrgCode : insertedUserId;
            
        }

        public async Task<Users> GetUserByEmailAsync(string email)
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

        public async Task<Users> GetUserByEmailAndOrgCodeAsync(string email,string orgCode)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.UserName,Operator.Eq,email),
                        Predicates.Field<Users>(f=>f.OrgCode,Operator.Eq,orgCode),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }

        public async Task<bool> DeleteUserByEmailAndOrgCodeAsync(string email, string organizationCode)
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

        public async Task<List<Users>> GetAllUserAsync()
        {
            
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users";

                var users = dbInstance.Select<Users>(sql).ToList();
                return users;
            
          
        }

        public async Task<List<Users>> GetUserByOrgCodeAsync(string orgCode)
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users Where OrgCode=@orgCode";

                var users = dbInstance.Select<Users>(sql, new {orgCode}).ToList();
                return users;
            

        }

        public async Task<Users> GetUserByIdAsync(int Id)
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

        public async Task<bool> VerifyUserEmailAsync(string userToken, string Otp)
        {
            
                var res= await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Users>(f=>f.Token,Operator.Eq,userToken),
                        Predicates.Field<Users>(f=>f.VerificationToken,Operator.Eq,Otp),
                        Predicates.Field<Users>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
                if(res!=null)
                {
                    var dbInstance = GetDbInstance();
                    var tableName = new Table<Users>();
                    var updateQuery = new Query(tableName.TableName)
                            .AsUpdate(new
                            {
                                Verificationtoken = "",
                                IsVerified = true,
                                Token = "",
                                UpdatedOn = DateTime.UtcNow,
                                LastLoginTime = DateTime.UtcNow
                            })
                            .Where("Token", userToken);

                    await dbInstance.ExecuteAsync(updateQuery);
                    return true;
                }
                return false;
            
        }

        public async Task<bool> UpdateOtpAndTokenAsync(string otp,string token,string email)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Users>();
                var updateQuery = new Query(tableName.TableName)
                        .AsUpdate(new
                        {
                            Verificationtoken = otp,
                            Token = token,
                            UpdatedOn = DateTime.UtcNow,
                            LastLoginTime = DateTime.UtcNow
                        })
                        .Where("Username", email);

                await dbInstance.ExecuteAsync(updateQuery);
                return true;
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    Phone = request.Phone,
                    DOB = request.DOB,
                    IsDeleted = false
                })
                .Where("Username", request.Email);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<bool> UpdateChangePasswordAsync(ChangePasswordRequest request, string hashedPassword, byte[] salt)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Users>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                   Password= hashedPassword,
                   PasswordSalt= salt,
                    IsDeleted = false
                })
                .Where("Username", request.Email);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

    }

}
