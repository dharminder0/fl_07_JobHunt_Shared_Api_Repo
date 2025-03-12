namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesRepository : StaticBaseRepository<Resources>, IResourcesRepository
    {
        public ResourcesRepository(IConfiguration configuration): base(configuration)
        {
        
        }

        public async Task<bool> UpsertApplicants(ApplicationsRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Applications>();
            var query = new Query(tableName.TableName)
                   .Where("ResourceId", request.ResourceId)
                   .Where("RequirementId", request.RequirementId)
                   .Select("Id");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);
            if (existingOrgCode != null)
            {
                var updateQuery = new Query(tableName.TableName).AsUpdate(
                    new
                    {
                        ResourceId = request.ResourceId,
                        RequirementId = request.RequirementId,
                        Comment = request.Comment,
                        Status = request.Status,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = Convert.ToInt32(request.UserId)
                    }).Where("Id", existingOrgCode);
                await dbInstance.ExecuteAsync(updateQuery);
                return true;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(
                new
                {
                    ResourceId = request.ResourceId,
                    RequirementId = request.RequirementId,
                    Comment = request.Comment,
                    Status = request.Status,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt32(request.UserId)
                });
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }
    }
}
