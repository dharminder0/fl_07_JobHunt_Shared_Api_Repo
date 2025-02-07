using AutoMapper;
using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgLocationRepository:StaticBaseRepository<OrgLocation>,IOrgLocationRepository
    {
        public OrgLocationRepository(IConfiguration configuration):base(configuration) 
        {

        }

        public async Task<bool>UpsertLocation(OrgLocation location)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<OrgLocation>();
                var checkUserExist = new Query(tableName.TableName)
                      .Where("OrgCode", location.OrgCode)
                      .Where("City",location.City)
                      .Where("IsDeleted",false)
                      .Select("Id");

                var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
                if (existing != null)
                {
                    return true;
                }
                // Insert new user
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    OrgCode = location.OrgCode,
                    City = location.City,
                    State=location.State,
                    CreatedOn= DateTime.UtcNow,
                    IsDeleted = false
                });

                await dbInstance.ExecuteScalarAsync<string>(insertQuery);
              

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<OrgLocation>> GetOrgLocation(string orgCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM OrgLocation Where OrgCode=@orgCode And IsDeleted='0'";

                var orgdata = dbInstance.Select<OrgLocation>(sql, new {orgCode}).ToList();
                return orgdata;
            }
            catch (Exception ex)
            {
                return new List<OrgLocation>();
            }
        }

        public async Task<bool> DeleteOrgLocationAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgLocation>();
            var checkUserExist = new Query(tableName.TableName)
                  .Where("OrgCode", orgCode)
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
