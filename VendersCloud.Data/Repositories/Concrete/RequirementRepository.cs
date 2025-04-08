using Newtonsoft.Json;
using VendersCloud.Common.Extensions;

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
                    request.ClientCode,
                    request.Remarks,
                    request.Status,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false,
                    skills = string.Join(",", request.Skills)

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
                    request.ClientCode,
                    request.Duration,
                    request.Remarks,
                    request.Status,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = Convert.ToInt32(request.UserId),
                    IsDeleted = false,
                    UniqueId = uniqueId,
                    skills = string.Join(",", request.Skills)
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
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = Convert.ToInt32(request.UserId),
                        IsDeleted = false,
                        skills = string.Join(",", request.Skills)
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
                        CreatedBy = Convert.ToInt32(request.UserId),
                        IsDeleted = false,
                        skills = string.Join(",", request.Skills)
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

        public async Task<List<Requirement>> GetRequirementByUserIdAsync(List<int> UserId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where CreatedBy In@UserId and IsDeleted<>1";
            var requirementData = dbInstance.Select<Requirement>(sql, new { UserId }).ToList();
            return requirementData;
        }
        public async Task<List<Requirement>> GetRequirementListByIdAsync(string requirementId)
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and UniqueId=@requirementId";

                var list = dbInstance.Select<Requirement>(sql, new { requirementId}).ToList();
                return list;
        }

        public async Task<IEnumerable<Requirement>> GetRequirementByIdAsync(List<int> requirementId)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and Id In @requirementId";

            var data = dbInstance.Select<Requirement>(sql, new { requirementId });
            return data;

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

        public async Task<bool> UpdateHotByIdAsync(string requirementUniqueId, int hot)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            var insertQuery = new Query(tableName.TableName)
                .AsUpdate(new
                {
                    Hot = hot,
                    IsDeleted = false
                })
                .Where("UniqueId", requirementUniqueId);
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

        public async Task<List<Requirement>> GetRequirementsListByVisibilityAsync(SearchRequirementRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.Title LIKE @searchText OR r.Description LIKE @searchText OR r.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.LocationType != null && request.LocationType.Any())
            {
                predicates.Add("r.LocationType IN @locationTypes");
                parameters.Add("locationTypes", request.LocationType);
            }

            if (!string.IsNullOrEmpty(request.UserId) && request.RoleType.Any() && request.RoleType != null)
            {
                var rolePlaceholders = string.Join(", ", request.RoleType.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM UserProfiles op WHERE  op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.RoleType.Count; i++)
                {
                    parameters.Add($"Role{i}", request.RoleType[i]);
                }
                parameters.Add("IsDeleted", false);
            }
            if (request.Status != null && request.Status.Any())
            {
                predicates.Add("r.Status IN @statuses");
                parameters.Add("statuses", request.Status);
            }

            if (request.ClientCode != null && request.ClientCode.Any())
            {
                predicates.Add("r.ClientCode IN @clientCodes");
                parameters.Add("clientCodes", request.ClientCode);
            }

            predicates.Add("r.IsDeleted = 0");
            predicates.Add("r.OrgCode <> @orgCode");
            parameters.Add("orgCode", request.OrgCode);
            predicates.Add("r.Visibility='3'");
            

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
SELECT * FROM Requirement r
{whereClause}
ORDER BY r.CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var requirements = (await multi.ReadAsync<Requirement>()).ToList();
            return requirements;
        }

        public async Task<List<Requirement>> GetRequirementsListAsync(SearchRequirementRequest request)
        {
            using var connection = GetConnection();
            var predicates = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                predicates.Add("(r.Title LIKE @searchText OR r.Description LIKE @searchText OR r.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.LocationType != null && request.LocationType.Any())
            {
                predicates.Add("r.LocationType IN @locationTypes");
                parameters.Add("locationTypes", request.LocationType);
            }

            if (!string.IsNullOrEmpty(request.UserId) && request.RoleType.Any() && request.RoleType != null)
            {
                var rolePlaceholders = string.Join(", ", request.RoleType.Select((role, index) => $"@Role{index}"));
                predicates.Add($"EXISTS (SELECT 1 FROM UserProfiles op WHERE op.UserId = @UserId AND op.ProfileId IN ({rolePlaceholders}))");

                for (int i = 0; i < request.RoleType.Count; i++)
                {
                    parameters.Add($"Role{i}", request.RoleType[i]);
                }
                parameters.Add("IsDeleted", false);
                parameters.Add("UserId", Convert.ToInt32(request.UserId));
            }
            if (request.Status != null && request.Status.Any())
            {
                predicates.Add("r.Status IN @statuses");
                parameters.Add("statuses", request.Status);
            }

            if (request.ClientCode != null && request.ClientCode.Any())
            {
                predicates.Add("r.ClientCode IN @clientCodes");
                parameters.Add("clientCodes", request.ClientCode);
            }

            predicates.Add("r.IsDeleted = 0");
            predicates.Add("r.OrgCode = @orgCode");
            parameters.Add("orgCode", request.OrgCode);

            string whereClause = predicates.Any() ? "WHERE " + string.Join(" AND ", predicates) : "";

            string query = $@"
SELECT * FROM Requirement r
{whereClause}
ORDER BY r.CreatedOn DESC;";

            using var multi = await connection.QueryMultipleAsync(query, parameters);
            var requirements = (await multi.ReadAsync<Requirement>()).ToList();
            return requirements;
        }

        public async Task<CompanyDashboardCountResponse> GetCountsAsync(string orgCode)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("orgCode", orgCode);

            string query = @"
                SELECT 
            (SELECT SUM(Positions) FROM Requirement WHERE Status = 1 AND OrgCode = @orgCode) AS OpenPositions,
        
            (SELECT COUNT(*) FROM Requirement WHERE Hot = 1 AND Status = 1 AND OrgCode = @orgCode) AS HotRequirements,
        
            (SELECT COUNT(*) FROM Applications WHERE Status IN (5, 6) 
             AND RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS InterviewScheduled,
        
            (SELECT COUNT(*) FROM Applications WHERE Status IN (2) 
             AND RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS CandidatesToReview,
        
            (SELECT COUNT(*) FROM Applications 
             WHERE RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS TotalApplicants,
        
            (SELECT COUNT(*) 
             FROM Requirement r 
             WHERE r.OrgCode = @orgCode
             AND NOT EXISTS (
                 SELECT 1 FROM Applications a WHERE a.RequirementId = r.Id
             )) AS NoApplications
        ";

            return await connection.QueryFirstOrDefaultAsync<CompanyDashboardCountResponse>(query, parameters)
                   ?? new CompanyDashboardCountResponse();
        }

        public async Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode ,string userId)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("orgCode", orgCode);
            parameters.Add("userId", userId);

            string query = @"SELECT (SELECT SUM(Positions) FROM Requirement WHERE Status = 1 ) AS OpenPositions,
                            (SELECT COUNT(*) FROM Requirement WHERE Hot = 1 AND Status = 1 ) AS HotRequirements,
                            (SELECT COUNT(*) FROM Applications WHERE Status IN (5, 6) And CreatedBy=@userId) AS InterviewScheduled,
                            (SELECT COUNT(*) FROM Applications WHERE Status IN (2) And CreatedBy=@userId) AS CandidatesToReview,
                            (SELECT COUNT(*) FROM Applications WHERE  CreatedBy=@userId)  AS TotalApplicants";

            return await connection.QueryFirstOrDefaultAsync<CompanyDashboardCountResponse>(query, parameters)
                   ?? new CompanyDashboardCountResponse();
        }

        public async Task<List<dynamic>>GetActivePositionsByOrgCodeAsync(string orgCode,string userId)
        { 
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            var sql = @"SELECT ClientCode,CreatedBy, SUM(Positions) AS TotalPositions 
                        FROM Requirement 
                        WHERE  (OrgCode = @orgCode OR (CreatedBy=@userId OR UpdatedBy=@userId))  and Status<>3 
                        GROUP BY ClientCode, CreatedBy
                        ORDER BY TotalPositions DESC;";
            return dbInstance.Select<dynamic>(sql, new { orgCode,userId }).ToList();
        }

        public async Task<List<dynamic>> GetOrgTotalPlacementAndRequirementIdAsync(CompanyGraphRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            string requirementQuery = @" SELECT 
                   OrgCode, 
                   LEFT(DATENAME(WEEKDAY, CreatedOn), 3) AS WeekDay,  
                   SUM(Positions) AS TotalPositions, 
                   STRING_AGG(Id, ',') AS RequirementIds
               FROM Requirement
               WHERE OrgCode = @orgCode 
                   AND CreatedOn BETWEEN  @StartDate AND @EndDate and Status<>3 and ISDeleted<>1
               GROUP BY OrgCode, LEFT(DATENAME(WEEKDAY, CreatedOn), 3)
               ORDER BY 
                   CASE 
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Mon' THEN 1
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Tue' THEN 2
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Wed' THEN 3
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Thu' THEN 4
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Fri' THEN 5
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Sat' THEN 6
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Sun' THEN 7
                   END";
            return dbInstance.Select<dynamic>(requirementQuery, new { request.OrgCode, request.StartDate, request.EndDate }).ToList();
        }

        public async Task<List<dynamic>> GetVendorTotalPlacementAndRequirementIdAsync(VendorGraphRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            string requirementQuery = @" SELECT 
                   OrgCode, 
                   LEFT(DATENAME(WEEKDAY, CreatedOn), 3) AS WeekDay,  
                   SUM(Positions) AS TotalPositions, 
                   STRING_AGG(Id, ',') AS RequirementIds
               FROM Requirement
               WHERE OrgCode = @orgCode 
                   AND CreatedOn BETWEEN  @StartDate AND @EndDate and Status<>3 AND ISDeleted<>1 AND CreatedBy=@UserId
               GROUP BY OrgCode, LEFT(DATENAME(WEEKDAY, CreatedOn), 3)
               ORDER BY 
                   CASE 
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Mon' THEN 1
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Tue' THEN 2
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Wed' THEN 3
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Thu' THEN 4
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Fri' THEN 5
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Sat' THEN 6
                       WHEN LEFT(DATENAME(WEEKDAY, CreatedOn), 3) = 'Sun' THEN 7
                   END";
            return dbInstance.Select<dynamic>(requirementQuery, new { request.OrgCode, request.StartDate, request.EndDate,request.UserId }).ToList();
        }

        public async Task<dynamic> GetRequirementCountAsync(CompanyGraphRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            string requirementQuery = @" SELECT 
                COUNT(CASE WHEN status = 1 THEN 1 END) AS [Open],
                COUNT(CASE WHEN status = 2 THEN 1 END) AS [Onhold],
                COUNT(CASE WHEN status = 3 THEN 1 END) AS [Closed]
            FROM Requirement  WHERE OrgCode = @orgCode 
                   AND CreatedOn BETWEEN  @StartDate AND @EndDate  AND ISDeleted<>1;";
            return dbInstance.Select<dynamic>(requirementQuery, new { request.OrgCode, request.StartDate, request.EndDate }).ToList();
        }
        public async Task<dynamic> GetVendorRequirementCountAsync(VendorGraphRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            string requirementQuery = @" SELECT 
                COUNT(CASE WHEN status = 1 THEN 1 END) AS [Open],
                COUNT(CASE WHEN status = 2 THEN 1 END) AS [Onhold],
                COUNT(CASE WHEN status = 3 THEN 1 END) AS [Closed]
            FROM Requirement  WHERE OrgCode = @orgCode 
                   AND CreatedOn BETWEEN  @StartDate AND @EndDate  AND ISDeleted<>1 AND CreatedBy=@UserId;";
            return dbInstance.Select<dynamic>(requirementQuery, new { request.OrgCode, request.StartDate, request.EndDate ,request.UserId}).ToList();
        }

    }
}
