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

    }
}
