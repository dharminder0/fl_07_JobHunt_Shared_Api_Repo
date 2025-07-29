namespace VendersCloud.Data.Repositories.Concrete
{
    public class SkillResourcesMappingRepository : StaticBaseRepository<SkillResourcesMapping>, ISkillResourcesMappingRepository
    {
        public SkillResourcesMappingRepository(IConfiguration configuration) : base(configuration)
        {
                
        }

        public async Task<int> UpsertSkillRequirementMappingAsync(int skillId, int resourceId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<SkillResourcesMapping>();
            var query = new Query(tableName.TableName)
                   .Where("SkillId", skillId)
                   .Where("ResourceId", resourceId)
                   .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<int>(query);
            if (existingOrgCode > 0)
            {
                return existingOrgCode;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                SkillId = skillId,
                ResourceId = resourceId
            });
            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<int>(insertQuery);
            return insertedOrgCode;

        }
    }
}
