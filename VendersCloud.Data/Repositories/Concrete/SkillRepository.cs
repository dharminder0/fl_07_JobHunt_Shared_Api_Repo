namespace VendersCloud.Data.Repositories.Concrete
{
    public class SkillRepository : StaticBaseRepository<Skills>, ISkillRepository
    {
        public SkillRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<List<Skills>> SkillUpsertAsync(List<string> skillNames)
        {
            var result = new List<Skills>();

            foreach (var name in skillNames)
            {
                
                var existingSkill = await GetSkillByNameAsync(name);

                if (existingSkill != null)
                { 
                    result.Add(existingSkill);
                }
                else
                {
                    var newSkill = new Skills
                    {
                        SkillName = name
                    };

                    var insertedSkill = await UpsertAsync(newSkill); 
                    result.Add(insertedSkill);
                }
            }

            return result;
        }

        public async Task<Skills> GetSkillByNameAsync(string name)
        {
            var dbInstance = GetDbInstance();
            var table = new Table<Skills>();

            var query = new Query(table.TableName)
                .Where("SkillName", name)
                .Select("Id", "SkillName");

            var skill = await dbInstance.FirstOrDefaultAsync<Skills>(query); 

            return skill;
        }

        public async Task<List<string>> GetAllSkillNamesAsync(List<int> skillIds)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT SkillName FROM Skills WHERE Id IN @Ids";
            var namedata = await dbInstance.SelectAsync<string>(sql, new { Ids = skillIds });
            return namedata.ToList();
        }


    }
}
