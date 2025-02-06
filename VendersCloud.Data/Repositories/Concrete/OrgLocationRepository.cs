using AutoMapper;
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
                      .Where("City", location.City)
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
                }).Select("Id");

                var res= await dbInstance.ExecuteScalarAsync<string>(insertQuery);
                if(res!=null)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
