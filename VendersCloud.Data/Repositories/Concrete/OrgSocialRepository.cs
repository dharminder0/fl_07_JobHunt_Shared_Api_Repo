using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgSocialRepository:StaticBaseRepository<OrgSocial>,IOrgSocialRepository
    {
        public OrgSocialRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<bool> UpsertSocialProfile(OrgSocial social)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<OrgSocial>();

                // Check if the user already exists
                var checkUserExist = new Query(tableName.TableName)
                      .Where("Platform", social.Platform)
                      .Where("Name", social.Name)
                      .Where("IsDeleted",false)
                      .Select("Id");

                var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);

                if (existing != null)
                {
                    // Update the existing record
                    var updateQuery = new Query(tableName.TableName).AsUpdate(new
                    {
                        URL = social.URL
                    })
                    .Where("Platform", social.Platform)
                    .Where("Name", social.Name);

                    await dbInstance.ExecuteAsync(updateQuery);  // Execute the update
                }
                else
                {
                    // Insert a new record
                    var insertQuery = new Query(tableName.TableName).AsInsert(new
                    {
                        OrgCode = social.OrgCode,
                        Platform = social.Platform,
                        Name = social.Name,
                        URL = social.URL
                    });

                    await dbInstance.ExecuteScalarAsync<string>(insertQuery); // Execute the insert
                }

                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return false;
            }
        }

        public async Task<List<OrgSocial>> GetOrgSocialProfile(string orgCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM OrgSocial Where OrgCode=@orgCode And IsDeleted='0'";

                var orgdata = dbInstance.Select<OrgSocial>(sql, new {orgCode}).ToList();
                return orgdata;
            }
            catch (Exception ex)
            {
                return new List<OrgSocial>();
            }
        }

        public async Task<bool> DeleteOrgSocialAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgSocial>();
            var checkUserExist = new Query(tableName.TableName)
                  .Where("OrgCode", orgCode)
                  .Where("IsDeleted",false)
                  .Select("Id");

            var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
            if (existing != null)
            {
                var updateQuery = new Query(tableName.TableName)
                     .AsUpdate(new
                     {
                         IsDeleted = true
                     })
                     .Where("OrgCode", orgCode);

                await dbInstance.ExecuteAsync(updateQuery);
            }
            return true;

        }
    }
}
