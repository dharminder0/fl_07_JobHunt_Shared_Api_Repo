using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgRelationshipsRepository : StaticBaseRepository<OrgRelationships>, IOrgRelationshipsRepository
    {
        public OrgRelationshipsRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<bool> AddOrgRelationshipDataAsync(string orgCode,string relatedOrgCode,string relationshipType,int status,int createdBy)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgRelationships>();

            var query = new Query(tableName.TableName)
                .Where("IsDeleted", false)
                .Where("OrgCode", orgCode)
                .Where("RelatedOrgCode",relatedOrgCode)
                .Where("relationshipType",relationshipType)
                .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);
            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                return false;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                OrgCode = orgCode,
                RelatedOrgCode = relatedOrgCode,
                RelationshipType = relationshipType,
                Status = status,
                CreatedBy=createdBy,
                UpdatedBy= createdBy,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false
            });

            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(insertQuery);
            return true;
        }
    }
}
 