namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgProfilesRepository : StaticBaseRepository<OrgProfiles>, IOrgProfilesRepository
    {
        public OrgProfilesRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<bool> AddOrganizationProfileAsync(string orgCode, int profileId)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<OrgProfiles>();
            var checkUserExist = new Query(tableName.TableName)
                  .Where("OrgCode", orgCode)
                  .Where("ProfileId", profileId)
                  .Select("ProfileId");

            var existing = await dbInstance.ExecuteScalarAsync<string>(checkUserExist);
            if (existing != null)
            {
                return true;
            }
            // Insert new user
            var insertQuery = new Query(tableName.TableName).AsInsert(new
            {
                OrgCode = orgCode,
                ProfileId = profileId,
                IsDeleted = false
            });

            await dbInstance.ExecuteScalarAsync<string>(insertQuery);

            return true;

        }

        public async Task<List<OrgProfiles>> GetOrgProfilesByOrgCodeAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM OrgProfiles Where OrgCode=@orgCode";

            var profile = dbInstance.Select<OrgProfiles>(sql, new {orgCode}).ToList();
            return profile;
        }



        public async Task<PaginationDto<Organization>> SearchOrganizationsDetails(SearchRequest request)
        {
            using var connection = GetConnection(); // Ensure this returns IDbConnection
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                predicates.Add("(o.OrgName LIKE @searchText OR o.Description LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.Technology != null && request.Technology.Any())
            {
                var techPlaceholders = string.Join(", ", request.Technology.Select((tech, index) => $"@Tech{index}"));
                predicates.Add($"EXISTS ( ))");

                for (int i = 0; i < request.Technology.Count; i++)
                {
                    parameters.Add($"Tech{i}", request.Technology[i]);
                }
            }
            if (request.Role != 0)
            {
                predicates.Add($@"
        EXISTS (
            SELECT 1 
            FROM OrgProfiles op 
            WHERE op.OrgCode = o.OrgCode 
            AND op.ProfileId = @RoleId
        )");
                parameters.Add("RoleId", request.Role);
            }


            if (request.Resource != null && request.Resource.Any())
            {
                var resourcePlaceholders = string.Join(", ", request.Resource.Select((r, index) => $"@Resource{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM Requirement op WHERE op.OrgCode = o.OrgCode AND op.LocationType IN ({resourcePlaceholders}))");

                for (int i = 0; i < request.Resource.Count; i++)
                {
                    parameters.Add($"Resource{i}", request.Resource[i]);
                }
            }


            if (request.Strength != null && request.Strength.Any())
            {
                var strengthConditions = new List<string>();
                for (int i = 0; i < request.Strength.Count; i++)
                {
                    if (int.TryParse(request.Strength[i], out int strength))
                    {
                        int minStrength = 0, maxStrength = int.MaxValue;

                        if (strength == 0) { minStrength = 0; maxStrength = 50; }
                        else if (strength == 50) { minStrength = 50; maxStrength = 100; }
                        else if (strength == 100) { minStrength = 100; maxStrength = 200; }
                        else if (strength == 200) { minStrength = 200; maxStrength = 500; }
                        else if (strength == 500) { minStrength = 500; maxStrength = int.MaxValue; }

                        strengthConditions.Add($"(o.EmpCount BETWEEN @minStrength{i} AND @maxStrength{i})");
                        parameters.Add($"minStrength{i}", minStrength);
                        parameters.Add($"maxStrength{i}", maxStrength);
                    }
                }
                if (strengthConditions.Any())
                {
                    predicates.Add($"({string.Join(" OR ", strengthConditions)})");
                }
            }

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
        SELECT * FROM Organization o
        {whereClause}
        ORDER BY o.CreatedOn DESC
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

        SELECT COUNT(*) FROM Organization o {whereClause};";

            parameters.Add("offset", (request.Page - 1) * request.PageSize);
            parameters.Add("pageSize", request.PageSize);

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var organizations = (await multi.ReadAsync<Organization>()).ToList();
           
         
            int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

            return new PaginationDto<Organization>
            {
                Count = totalRecords,
                Page = request.Page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                List = organizations
            };
        }


    }
}
