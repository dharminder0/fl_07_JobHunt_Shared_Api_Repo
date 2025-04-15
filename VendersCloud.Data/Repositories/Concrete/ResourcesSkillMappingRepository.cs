namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesSkillMappingRepository : StaticBaseRepository<ResourcesSkillMapping>, IResourcesSkillMappingRepository
    {
        public ResourcesSkillMappingRepository(IConfiguration configuration) : base(configuration)
        {
                
        }

        public async Task<int> UpsertSkillRequirementMappingAsync(int skillId, int resourcesId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<ResourcesSkillMapping>();
            var query = new Query(tableName.TableName)
                   .Where("SkillId", skillId)
                   .Where("ResourcesId", resourcesId)
                   .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<int>(query);
            if (existingOrgCode > 0)
            {
                return existingOrgCode;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                SkillId = skillId,
                ResourcesId = resourcesId
            });
            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<int>(insertQuery);
            return insertedOrgCode;

        }
    }
}
