using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class BenchRepository : StaticBaseRepository<Resources>, IBenchRepository
    {
        public BenchRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<bool> UpsertBenchMembersAsync(BenchRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Resources>();
            var query = new Query(tableName.TableName)
                   .Where("IsDeleted", false)
                   .Where("Title", request.Title)
                   .Where("FirstName", request.FirstName)
                   .Where("LastName", request.LastName)
                   .Select("OrgCode");

            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);
            if (existingOrgCode != null)
            {
                var updateQuery = new Query(tableName.TableName).AsUpdate(
                    new
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Title = request.Title,
                        Email = request.Email,
                        Phone = request.Phone,
                        Linkedin = request.Linkedin,
                        CV = request.CV,
                        Availability = request.Availability,
                        OrgCode = request.OrgCode,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = Convert.ToInt32(request.UserId),
                        IsDeleted = false
                    }).Where("OrgCode", request.OrgCode);
                await dbInstance.ExecuteAsync(updateQuery);
                return true;
            }
            var insertQuery = new Query(tableName.TableName).AsInsert(
                new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Title = request.Title,
                    Email = request.Email,
                    Phone = request.Phone,
                    Linkedin = request.Linkedin,
                    CV = request.CV,
                    Availability = request.Availability,
                    OrgCode = request.OrgCode,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false
                });
            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

        public async Task<List<Resources>>GetBenchResponseListAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Resources Where IsDeleted<>1 and OrgCode=@orgCode";

            var list = dbInstance.Select<Resources>(sql, new { orgCode }).ToList();
            return list;
        }
    }
}
