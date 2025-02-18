using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementRepository : StaticBaseRepository<Requirement>,IRequirementRepository
    {
        public RequirementRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<string> RequirementUpsertAsync(RequirementRequest request,string uniqueId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>().TableName;
            var sql = "SELECT * FROM Requirement WHERE Title = @Title AND OrgCode = @OrgCode";

            // Trim and validate input data
            var cleanedTitle = request.Title.Trim();
            var cleanedOrgCode = request.OrgCode.Trim();

            var response = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
            string result = "";

            if (response.Any())
            {
                // Update query
                var updateQuery = new Query(tableName).AsUpdate(new
                {
                    Title = cleanedTitle,
                    OrgCode = cleanedOrgCode,
                    request.Description,
                    request.Experience,
                    request.Budget,
                    request.Positions,
                    request.LocationType,
                    request.Location,
                    request.Duration,
                    request.ClientId,
                    request.Remarks,
                    request.Status,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                    UpdatedBy = "",
                    IsDeleted = false,
                    UniqueId= uniqueId,
                }).Where("Title", cleanedTitle).Where("OrgCode", cleanedOrgCode);

                await dbInstance.ExecuteAsync(updateQuery);

                // Fetch the Id
                var idResponse = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                result = idResponse.FirstOrDefault()?.Id.ToString() ?? string.Empty;
            }
            else
            {
                // Insert query
                var insertQuery = new Query(tableName).AsInsert(new
                {
                    Title = cleanedTitle,
                    OrgCode = cleanedOrgCode,
                    request.Description,
                    request.Experience,
                    request.Budget,
                    request.Positions,
                    request.LocationType,
                    request.Location,
                    request.ClientId,
                    request.Duration,
                    request.Remarks,
                    request.Status,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = "",
                    UpdatedBy = "",
                    IsDeleted = false,
                    UniqueId = uniqueId,
                });

                await dbInstance.ExecuteAsync(insertQuery);

                // Fetch the Id
                var idResponse = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                result = idResponse.FirstOrDefault()?.UniqueId.ToString() ?? string.Empty;
            }

            return result;
        }




        public async Task<bool> RequirementUpsertV2Async(RequirementDto request, string uniqueId)
        {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Requirement>();
                var sql = "SELECT * FROM Requirement WHERE Title=@Title AND OrgCode=@OrgCode";

                // Trim and validate input data
                var cleanedTitle = request.Title.Trim();
                var cleanedOrgCode = request.OrgCode.Trim();

                var response = await dbInstance.SelectAsync<Requirement>(sql, new { Title = cleanedTitle, OrgCode = cleanedOrgCode });
                if (response.Any())
                {
                    var updateQuery = new Query(tableName.TableName).AsUpdate(new
                    {
                        Title = cleanedTitle,
                        OrgCode = cleanedOrgCode,
                        request.Description,
                        request.Experience,
                        request.Budget,
                        request.Positions,
                        request.LocationType,
                        request.Location,
                        request.Duration,
                        request.ClientCode,
                        request.Remarks,
                        request.Visibility,
                        request.Hot,
                        request.Status,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        CreatedBy = "",
                        UpdatedBy = "",
                        IsDeleted = false
                    }).Where("Title", cleanedTitle).Where("OrgCode", cleanedOrgCode);
                    await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                }
                else
                {
                    var insertQuery = new Query(tableName.TableName).AsInsert(new
                    {
                        Title = cleanedTitle,
                        OrgCode = cleanedOrgCode,
                        request.Description,
                        request.Experience,
                        request.Budget,
                        request.Positions,
                        request.LocationType,
                        request.Location,
                        request.ClientCode,
                        request.Duration,
                        request.Remarks,
                        request.Visibility,
                        request.Hot,
                        request.Status,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        CreatedBy = "",
                        UpdatedBy = "",
                        IsDeleted = false
                    });
                    await dbInstance.ExecuteScalarAsync<string>(insertQuery);
                }
                return true;
        }

        public async Task<bool> DeleteRequirementAsync(int requirementId, string orgCode)
        {
           
                var dbInstance = GetDbInstance();
                var tableName = new Table<Requirement>();
                var sql = "SELECT * FROM Requirement WHERE Id=@Id AND OrgCode=@OrgCode";

                // Trim and validate input data
                var Id = requirementId;
                var cleanedOrgCode = orgCode.Trim();

                var response = await dbInstance.SelectAsync<Requirement>(sql, new { Id = Id, OrgCode = cleanedOrgCode });
                if (response.Any())
                {
                    var updateQuery = new Query(tableName.TableName).AsUpdate(new
                    { 
                        IsDeleted = true
                    }).Where("Id", Id).Where("OrgCode", cleanedOrgCode);
                    await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                    return true;
                }
                return false;
            
        }

        public async Task<List<Requirement>> GetRequirementListAsync()
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Requirement Where IsDeleted<>1 Order By 1 Desc";

                var list = dbInstance.Select<Requirement>(sql).ToList();
                return list;
            
         
        }

        public async Task<List<Requirement>> GetRequirementListByIdAsync(string requirementId)
        {
           
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and UniqueId=@requirementId";

                var list = dbInstance.Select<Requirement>(sql, new { requirementId}).ToList();
                return list;
           
        }

        public async Task<bool> UpdateStatusByIdAsync(int requirementId, int status)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    Status=status,
                    IsDeleted = false
                })
                .Where("Id", requirementId);
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<List<Requirement>> GetRequirementByOrgCodeAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and OrgCode=@orgCode";

            var list = dbInstance.Select<Requirement>(sql, new { orgCode }).ToList();
            return list;

        }
    }
}
