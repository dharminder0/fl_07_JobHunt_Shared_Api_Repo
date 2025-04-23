using Newtonsoft.Json;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class BenchRepository : StaticBaseRepository<Resources>, IBenchRepository
    {
        public BenchRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> UpsertBenchMembersAsync(BenchRequest request)
        {
            string serializedCv = JsonConvert.SerializeObject(request.cv);
            var dbInstance = GetDbInstance();
            var tableName = new Table<Resources>();

            var existsQuery = new Query(tableName.TableName)
                .Where("IsDeleted", false)
                .Where("Id", request.Id)
                .Select("Id");

            bool exists = await dbInstance.ExecuteScalarAsync<int?>(existsQuery) != null;

            if (exists)
            {
                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Title = request.Title,
                    Email = request.Email,
                    CV = serializedCv,
                    Availability = request.Availability,
                    OrgCode = request.OrgCode,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = Convert.ToInt32(request.UserId),
                    SkillsEmbedding="",
                    IsDeleted = false
                }).Where("Id", request.Id);

                await dbInstance.ExecuteAsync(updateQuery);
                return request.Id;
            }
            else
            {
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Title = request.Title,
                    Email = request.Email,
                    CV = serializedCv,
                    Availability = request.Availability,
                    OrgCode = request.OrgCode,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false
                });

                await dbInstance.ExecuteAsync(insertQuery);
                var query2 = new Query(tableName.TableName).Where("FirstName", request.FirstName).Where("OrgCode", request.OrgCode)
                .Select("Id");

                var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(query2);
                var res = Convert.ToInt32(insertedOrgCode);
                return res;
            }
        }


        public async Task<List<Resources>> GetBenchResponseListAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Resources Where IsDeleted<>1 and OrgCode=@orgCode";

            var list = dbInstance.Select<Resources>(sql, new { orgCode }).ToList();
            return list;
        }

        public async Task<IEnumerable<Resources>> GetBenchResponseListByIdAsync(List<int> benchId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Resources Where IsDeleted<>1 and Id In @benchId";

            var list = dbInstance.Select<Resources>(sql, new { benchId }).ToList();
            return list;
        }

        public async Task<IEnumerable<Resources>> GetBenchResponseByIdAsync(int benchId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Resources Where IsDeleted<>1 and Id = @benchId";

            var list = dbInstance.Select<Resources>(sql, new { benchId }).ToList();
            return list;
        }
        public async Task<List<Resources>> GetBenchListBySearchAsync(BenchSearchRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.FirstName LIKE @searchText OR r.FirstName Like @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }
            if (request.Availability != null && request.Availability.Any(a => a > 0))
            {
                predicates.Add("(r.Availability In @availability)");
                parameters.Add("availability", request.Availability);
            }
            predicates.Add("r.IsDeleted=0");
            predicates.Add("r.OrgCode=@orgCode");
            parameters.Add("orgCode", request.OrgCode);
            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";
            string query = $@" Select * From Resources r {whereClause} Order By r. CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var response = (await multi.ReadAsync<Resources>()).ToList();
            return response;
        }

        public async Task<bool> UpsertAvtarbyIdAsync(int id, string avtar)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Resources>();
            var existsQuery = new Query(tableName.TableName)
               .Where("IsDeleted", false)
               .Where("Id", id)
               .Select("Id");

            bool exists = await dbInstance.ExecuteScalarAsync<int?>(existsQuery) != null;

            if (exists)
            {
                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                    Avtar = avtar,
                }).Where("Id", id);

                await dbInstance.ExecuteAsync(updateQuery);
                return true;
            }
            else
            {
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    Avtar = avtar,
                });
                await dbInstance.ExecuteAsync(insertQuery);
                return true;
            }
        }

        public async Task<IEnumerable<string>> GetAvtarByIdAsync(int benchId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT Avtar FROM Resources Where IsDeleted<>1 and Id = @benchId";
            var list = dbInstance.Select<string>(sql, new { benchId });
            return list;
        }
    }
}
