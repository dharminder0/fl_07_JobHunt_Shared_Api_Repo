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
                      .Select("Id");

                var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
                if(existing!=null)
                {
                    var deletequery = new Query(tableName.TableName).AsUpdate(new
                    {
                        IsDeleted=true
                    }).Where("UserId", userId);
                    await dbInstance.ExecuteScalarAsync<string>(deletequery);
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


        public async Task<List<UserProfiles>> GetProfileRole(int userId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM UserProfiles WHERE UserId=@userId and IsDeleted=0";

            var response = await dbInstance.SelectAsync<UserProfiles>(sql, new { userId });
            return response.ToList();
        }

    }
}
