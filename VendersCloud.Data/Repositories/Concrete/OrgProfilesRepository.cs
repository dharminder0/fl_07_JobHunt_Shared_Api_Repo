using Dapper;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrgProfilesRepository:StaticBaseRepository<OrgProfiles>, IOrgProfilesRepository
    {
        public OrgProfilesRepository(IConfiguration configuration):base(configuration)
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

            if (!string.IsNullOrEmpty(request.Technology))
            {
               
            }
            if (request.Role != null && request.Role.Any())
            {
                var rolePlaceholders = string.Join(", ", request.Role.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM OrgProfiles op WHERE op.OrgCode = o.OrgCode AND op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.Role.Count; i++)
                {
                    parameters.Add($"Role{i}", request.Role[i]);
                }
                parameters.Add("IsDeleted", false);
            }

            if (request.Resource.HasValue)
            {
                predicates.Add("EXISTS (SELECT 1 FROM Requirement op WHERE op.OrgCode = o.OrgCode AND op.LocationType = @resource)");
                parameters.Add("resource", request.Resource);
            }

            if (request.Strength.HasValue)
            {
                int minStrength = 0, maxStrength = int.MaxValue;

                if (request.Strength == 0)
                {
                    minStrength = 0;
                    maxStrength = 50;
                }
                else if (request.Strength == 50)
                {
                    minStrength = 50;
                    maxStrength = 100;
                }
                else if (request.Strength == 100)
                {
                    minStrength = 100;
                    maxStrength = 200;
                }
                else if (request.Strength == 200)
                {
                    minStrength = 200;
                    maxStrength = 500;
                }
                else if (request.Strength == 500)
                {
                    minStrength = 500;
                    maxStrength = int.MaxValue; // 500+ means no upper limit
                }

                predicates.Add("o.EmpCount BETWEEN @minStrength AND @maxStrength");
                parameters.Add("minStrength", minStrength);
                parameters.Add("maxStrength", maxStrength);
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
 