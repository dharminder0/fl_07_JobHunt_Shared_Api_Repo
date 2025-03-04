using Dapper;
using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
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

        public async Task<List<Resources>> GetBenchListBySearchAsync(BenchSearchRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if(!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.FirstName LIKE @searchText OR r.FirstName Like @searchText)");
                parameters.Add("searchText",$"%{request.SearchText}%");
            }

            predicates.Add("r.IsDeleted=0");
            predicates.Add("r.OrgCode=@orgCode");
            parameters.Add("orgCode", request.OrgCode);
            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";
            string query = $@" Select * From Resources r {whereClause} Order By r. CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query,parameters);
            var response = (await multi.ReadAsync<Resources>()).ToList();
            return response;
        }

       
    }
}
