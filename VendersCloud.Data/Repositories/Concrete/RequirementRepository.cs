using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
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

        public async Task<bool> RequirementUpsertAsync(RequirementRequest request)
        {
            try
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
                        request.ClientId,
                        request.Remarks,
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
                        request.ClientId,
                        request.Duration,
                        request.Remarks,
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
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                return false;
            }
        }




    }
}
