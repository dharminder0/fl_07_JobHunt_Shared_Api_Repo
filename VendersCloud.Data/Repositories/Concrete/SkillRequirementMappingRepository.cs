namespace VendersCloud.Data.Repositories.Concrete
{
    public class SkillRequirementMappingRepository : StaticBaseRepository<SkillRequirementMapping>, ISkillRequirementMappingRepository
    {
        public SkillRequirementMappingRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> UpsertSkillRequirementMappingAsync(int skillId, int requirementId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<SkillRequirementMapping>();
            var query = new Query(tableName.TableName)
                   .Where("SkillId", skillId)
                   .Where("RequirementId", requirementId)
                   .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<int>(query);
            if (existingOrgCode>0)
            {
                var updateQuery = new Query(tableName.TableName).AsInsert(new
                {
                    SkillId = skillId,
                    RequirementId = requirementId
                }).Where("Id", existingOrgCode);
                var updateOrgCode = await dbInstance.ExecuteScalarAsync<int>(updateQuery);
                return updateOrgCode;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                SkillId = skillId,
                RequirementId = requirementId
            });
            var insertedOrgCode = await dbInstance.ExecuteScalarAsync<int>(insertQuery);
            return insertedOrgCode; 

        }
        public async Task<List<SkillRequirementMapping>> GetSkillRequirementMappingAsync(int requirementId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<SkillRequirementMapping>();
            var query= @"Select * from SkillRequirementMapping where RequirementId=@requirementId";
            var orgdata = dbInstance.Select<SkillRequirementMapping>(query, new {requirementId}).ToList();
            return orgdata;
        }
        public async Task<List<int>> GetRequirementIdsBySkillMatchAsync(List<int> skillIds, int excludeId)
        {
            var dbInstance = GetDbInstance();
            var sql = @"
            SELECT DISTINCT RequirementId 
            FROM SkillRequirementMapping 
            WHERE SkillId IN @skillIds AND RequirementId != @excludeId";
            var result =  dbInstance.Select<int>(sql, new { skillIds, excludeId });
            return result.ToList();
        }
    }
}
