using AutoMapper;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserProfilesRepository:StaticBaseRepository<UserProfiles>, IUserProfilesRepository
    {
        public UserProfilesRepository(IConfiguration configuration):base(configuration)
        {
        }

        public async Task<bool> InsertUserProfileAsync(int userId, int profileId)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<UserProfiles>();
                var checkUserExist = new Query(tableName.TableName)
                      .Where("UserId", userId)
                      .Where("ProfileId", profileId)
                      .Select("ProfileId");

                var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
                if(existing!=null)
                {
                    return true;
                }
                // Insert new user
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    UserId = userId,
                    ProfileId = profileId,
                    IsDeleted = false
                });

                await dbInstance.ExecuteScalarAsync<string>(insertQuery);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public async Task<int> GetProfileRole(int userId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<UserProfiles>();

            // Check if the user exists in USERS table
            var query = new Query(tableName.TableName)
                .Where("UserId", userId)
                .Select("ProfileId");

           return await dbInstance.ExecuteScalarAsync<int>(query);
        }

    }
}
