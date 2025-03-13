namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesRepository : StaticBaseRepository<Resources>, IResourcesRepository
    {
        public ResourcesRepository(IConfiguration configuration): base(configuration)
        {
        
        }

        public async Task<bool> UpsertApplicants(ApplicationsRequest request,int Id)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Applications>();

            foreach (var item in request.ResourceId)
            {
                var query = new Query(tableName.TableName)
                    .Where("ResourceId", item)
                    .Where("RequirementId", Id)
                    .Select("Id");

                var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

                if (existingOrgCode != null)
                {
                    var updateQuery = new Query(tableName.TableName).AsUpdate(
                        new
                        {
                            ResourceId = item,
                            RequirementId = Id,
                            Comment = request.Comment,
                            Status = request.Status,
                            UpdatedOn = DateTime.UtcNow,
                            UpdatedBy = Convert.ToInt32(request.UserId)
                        }).Where("Id", existingOrgCode);

                    await dbInstance.ExecuteAsync(updateQuery);
                }
                else
                {
                    var insertQuery = new Query(tableName.TableName).AsInsert(
                        new
                        {
                            ResourceId = item,
                            RequirementId = Id,
                            Comment = request.Comment,
                            Status = request.Status,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = Convert.ToInt32(request.UserId)
                        });

                    await dbInstance.ExecuteAsync(insertQuery);
                }
            }

            return true;
        }

        public async Task<List<Applications>> GetApplicationsList()
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Applications";

            var applicationsData = dbInstance.Select<Applications>(sql).ToList();
            return applicationsData;
        }
    }
}
