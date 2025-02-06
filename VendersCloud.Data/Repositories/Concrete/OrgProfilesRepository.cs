using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgProfilesRepository:StaticBaseRepository<OrgProfiles>, IOrgProfilesRepository
    {
        public OrgProfilesRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<OrgProfiles>();
                var checkUserExist = new Query(tableName.TableName)
                      .Where("OrgCode", orgCode)
                      .Where("ProfileId", profileId)
                      .Select("ProfileId");

                var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
                if (existing != null)
                {
                    return true;
                }
                // Insert new user
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    OrgCode = orgCode,
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
    }
}
 