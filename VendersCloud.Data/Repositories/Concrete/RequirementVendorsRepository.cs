namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementVendorsRepository:StaticBaseRepository<RequirementVendors>, IRequirementVendorsRepository
    {
        public RequirementVendorsRepository(IConfiguration configuration):base(configuration)
        {

        }


        public async Task<bool> AddRequirementVendorsDataAsync(int requirementId, string orgCode)
        {
            if (requirementId <= 0)
            {
                throw new ArgumentOutOfRangeException("RequirementId is null");
            }
            if (string.IsNullOrEmpty(orgCode)) {
                throw new ArgumentOutOfRangeException("OrgCode is null");
            }

            var dbInstance = GetDbInstance();
            var tableName = new Table<RequirementVendors>();
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                RequirementId=requirementId,
                OrgCode=orgCode,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            });
            await dbInstance.ExecuteScalarAsync<string>(insertQuery);
            return true;


        }
        public async Task<List<int>> GetRequirementShareJobsAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT RequirementId FROM RequirementVendors Where OrgCode=@orgCode";

            var profile = dbInstance.Select<int>(sql, new { orgCode }).ToList();
            return profile;
        }
        public async Task<List<int>> GetRequirementShareJobsAsyncV2(List<string> orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT RequirementId FROM RequirementVendors Where OrgCode in @orgCode";

            var profile = dbInstance.Select<int>(sql, new { orgCode }).ToList();
            return profile;
        }
    }
}
