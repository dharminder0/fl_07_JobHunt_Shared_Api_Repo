using Newtonsoft.Json;
using VendersCloud.Common.Extensions;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class RequirementRepository : StaticBaseRepository<Requirement>,IRequirementRepository
    {
        public RequirementRepository(IConfiguration configuration):base(configuration)
        {
        }

        public async Task<Requirement> RequirementUpsertAsync(RequirementRequest request, string uniqueId)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Requirement>().TableName;

                // Clean and validate input
                var cleanedTitle = request.Title?.Trim();
                var cleanedOrgCode = request.OrgCode?.Trim();

                if (string.IsNullOrWhiteSpace(cleanedTitle) || string.IsNullOrWhiteSpace(cleanedOrgCode))
                {
                    throw new ArgumentException("Title and OrgCode are required.");
                }

                var selectByIdSql = "SELECT * FROM Requirement WHERE Id = @Id AND OrgCode = @OrgCode";
                var existingRequirement = await dbInstance.SelectAsync<Requirement>(selectByIdSql, new
                {
                    Id = request.Id,
                    OrgCode = cleanedOrgCode
                });

                Requirement result;

                if (existingRequirement.Any())
                {
                    // Update existing record
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
                        request.ClientCode,
                        request.Duration,
                        request.Remarks,
                        request.Status,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = Convert.ToInt32(request.UserId),
                        Embedding = "",
                        IsDeleted = false
                    })
                    .Where("Id", request.Id)
                    .Where("OrgCode", cleanedOrgCode);

                    await dbInstance.ExecuteAsync(updateQuery);

                    var updatedResult = await dbInstance.SelectAsync<Requirement>(selectByIdSql, new
                    {
                        Id = request.Id,
                        OrgCode = cleanedOrgCode
                    });

                    result = updatedResult.FirstOrDefault();
                }
                else
                {
                    // Insert new record
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
                        UniqueId = uniqueId
                    });

                    await dbInstance.ExecuteAsync(insertQuery);

                    var selectByUniqueIdSql = "SELECT * FROM Requirement WHERE UniqueId = @UniqueId AND OrgCode = @OrgCode";
                    var insertedResult = await dbInstance.SelectAsync<Requirement>(selectByUniqueIdSql, new
                    {
                        UniqueId = uniqueId,
                        OrgCode = cleanedOrgCode
                    });

                    result = insertedResult.FirstOrDefault();
                }

                return result;
            }
            catch (Exception ex)
            {
                // Optional: Log the exception here
                throw new ApplicationException("An unexpected error occurred while processing the requirement.", ex);
            }
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
                        Embedding="",
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
                        CreatedBy = Convert.ToInt32(request.UserId),
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

        public async Task<Requirement> GetRequirementByRequirementIdAsync(int requirementId)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Requirement Where IsDeleted<>1 and Id = @requirementId";

            var data = dbInstance.Select<Requirement>(sql, new { requirementId }).FirstOrDefault();
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
        public async Task<int> GetRequirementCountByOrgCodeAsync(string orgCode)
        {
            var dbInstance = GetDbInstance();
            var sql = " SELECT sum(Positions) FROM Requirement   Where IsDeleted<>1 and OrgCode=@orgCode";
            return ExecuteScalar<int>(sql, new { orgCode });
;

        }
        public async Task<int> GetRequirementCountByOrgCodeAsyncV2(string orgCode,string clientCode)
        {
            var dbInstance = GetDbInstance();
            var sql = " SELECT count(*) FROM Requirement WHERE IsDeleted <> 1  AND OrgCode = @orgCode   and clientCode=@clientCode  AND Status = 1 ";
            return ExecuteScalar<int>(sql, new { orgCode,clientCode });
            ;

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
                predicates.Add("(r.Title LIKE @searchText  OR r.ClientCode LIKE @searchText)");
                parameters.Add("searchText", $"%{request.SearchText}%");
            }

            if (request.LocationType != null && request.LocationType.Any())
            {
                predicates.Add("r.LocationType IN @locationTypes");
                parameters.Add("locationTypes", request.LocationType);
            }

            if (!string.IsNullOrEmpty(request.UserId) && request.RoleType != null && request.RoleType.Any())
            {
                var rolePlaceholders = string.Join(", ", request.RoleType.Select((_, i) => $"@Role{i}"));
                predicates.Add($@"
EXISTS (
    SELECT 1 FROM UserProfiles op
    WHERE op.UserId = @UserId
    AND op.ProfileId IN ({rolePlaceholders})
)");

                for (int i = 0; i < request.RoleType.Count; i++)
                {
                    parameters.Add($"Role{i}", request.RoleType[i]);
                }

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
            parameters.Add("orgCode", request.OrgCode);
            predicates.Add("r.OrgCode = @orgCode");

            var whereClause = predicates.Any()
                ? "WHERE " + string.Join(" AND ", predicates)
                : "";

            var query = $@"
SELECT * FROM Requirement r
{whereClause}
ORDER BY r.CreatedOn DESC;";

            var result = await connection.QueryAsync<Requirement>(query, parameters);
            return result.ToList();
        }


        public async Task<CompanyDashboardCountResponse> GetCountsAsync(string orgCode)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("orgCode", orgCode);

            string query = @"
                WITH LatestStatus AS (
    SELECT ash.ApplicantId, ash.Status, a.RequirementId,
           ROW_NUMBER() OVER (PARTITION BY ash.ApplicantId ORDER BY ash.ChangedOn DESC) AS rn
    FROM ApplicantStatusHistory ash
    JOIN Applications a ON a.id = ash.ApplicantId
)
, ApplicationWithStatus AS (
    SELECT ls.RequirementId, ls.ApplicantId, ls.Status
    FROM LatestStatus ls
    WHERE ls.rn = 1
)

SELECT 
    (SELECT SUM(Positions) 
     FROM Requirement 
     WHERE Status = 1 AND OrgCode = @orgCode) AS OpenPositions,

    (SELECT COUNT(*) 
     FROM Requirement 
     WHERE Hot = 1 AND Status = 1 AND OrgCode = @orgCode) AS HotRequirements,

    (SELECT COUNT(*) 
     FROM ApplicationWithStatus aws 
     WHERE aws.Status IN (5, 6, 7) 
     AND aws.RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS InterviewScheduled,

    (SELECT COUNT(*) 
     FROM ApplicationWithStatus aws 
     WHERE aws.Status = 2 
     AND aws.RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS CandidatesToReview,

    (SELECT COUNT(*) 
     FROM ApplicationWithStatus aws 
     WHERE aws.RequirementId IN (SELECT Id FROM Requirement WHERE OrgCode = @orgCode)) AS TotalApplicants,

       (SELECT COUNT(*) 
     FROM Requirement r
     LEFT JOIN Applications a ON a.RequirementId = r.Id
     WHERE r.OrgCode = @orgCode AND a.Id IS NULL) AS NoApplications

        ";

            return await connection.QueryFirstOrDefaultAsync<CompanyDashboardCountResponse>(query, parameters)
                   ?? new CompanyDashboardCountResponse();
        }

        public async Task<CompanyDashboardCountResponse> GetVendorsCountsAsync(string orgCode, string userId, int roleType)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("orgCode", orgCode);
            parameters.Add("userId", userId);

            string query = $@"
WITH LatestStatus AS (
    SELECT 
        ash.ApplicantId, 
        ash.Status, 
        a.RequirementId,
        ROW_NUMBER() OVER (PARTITION BY ash.ApplicantId ORDER BY ash.ChangedOn DESC) AS rn
    FROM ApplicantStatusHistory ash
    INNER JOIN Applications a ON a.Id = ash.ApplicantId
),
ApplicationWithStatus AS (
    SELECT 
        ls.RequirementId, 
        ls.ApplicantId, 
        ls.Status,
        a.CreatedBy
    FROM LatestStatus ls
    INNER JOIN Applications a ON a.Id = ls.ApplicantId
    WHERE ls.rn = 1
)
SELECT 
    (SELECT SUM(Positions) FROM Requirement WHERE Status = 1) AS OpenPositions,
    (SELECT COUNT(*) FROM Requirement WHERE Hot = 1 AND Status = 1) AS HotRequirements,
    (SELECT COUNT(*) FROM ApplicationWithStatus 
     WHERE Status IN (5, 6,7) AND CreatedBy = @userId) AS InterviewScheduled,
    (SELECT COUNT(*) FROM ApplicationWithStatus 
     WHERE Status = 2 AND CreatedBy = @userId) AS CandidatesToReview,
    (SELECT COUNT(*) FROM Applications WHERE CreatedBy = @userId) AS TotalApplicants";

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

            string requirementQuery = @"
    SET DATEFIRST 1; -- Set Monday as first day of the week
    SELECT 
        OrgCode, 
        LEFT(DATENAME(WEEKDAY, CAST(CreatedOn AS DATE)), 3) AS WeekDay,
        SUM(Positions) AS TotalPositions, 
        STRING_AGG(CAST(Id AS VARCHAR), ',') AS RequirementIds
    FROM Requirement
    WHERE OrgCode = @OrgCode 
        AND CAST(CreatedOn AS DATE) BETWEEN @StartDate AND @EndDate
        AND Status <> 3 
        AND IsDeleted <> 1
    GROUP BY 
        OrgCode, 
        CAST(CreatedOn AS DATE), 
        LEFT(DATENAME(WEEKDAY, CAST(CreatedOn AS DATE)), 3), 
        DATEPART(WEEKDAY, CAST(CreatedOn AS DATE))
    ORDER BY 
        DATEPART(WEEKDAY, CAST(CreatedOn AS DATE))";

            var result = await dbInstance.SelectAsync<dynamic>(requirementQuery, new
            {
                request.OrgCode,
                request.StartDate,
                request.EndDate
            });

            return result.ToList();
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
                   AND CreatedOn BETWEEN  @StartDate AND (SELECT DATEADD(day, 1, @EndDate)) and Status<>3 AND ISDeleted<>1 AND CreatedBy=@UserId
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

        public async Task<VendorRequirementCount> GetRequirementCountAsync(CompanyGraphRequest request)
        {
            var dbInstance = GetDbInstance();

            string requirementQuery = @"
        SELECT 
            COUNT(CASE WHEN status = 1 THEN 1 END) AS [Open],
            COUNT(CASE WHEN status = 2 THEN 1 END) AS [Onhold],
            COUNT(CASE WHEN status = 3 THEN 1 END) AS [Closed]
        FROM Requirement  
        WHERE OrgCode = @OrgCode 
          AND CreatedOn BETWEEN @StartDate AND DATEADD(day, 1, @EndDate)  
          AND IsDeleted <> 1;";

            var result = dbInstance.Select<VendorRequirementCount>(requirementQuery, new
            {
                request.OrgCode,
                request.StartDate,
                request.EndDate
            }).FirstOrDefault(); // return a single object

            return result ?? new VendorRequirementCount(); // handle null case
        }

        public async Task<VendorRequirementCount> GetVendorRequirementCountAsync(VendorGraphRequest request)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Requirement>();
            string requirementQuery = @" SELECT   
               COUNT(CASE WHEN status = 1 THEN 1 END) AS [Open],  
               COUNT(CASE WHEN status = 2 THEN 1 END) AS [Onhold],  
               COUNT(CASE WHEN status = 3 THEN 1 END) AS [Closed]  
           FROM Requirement  
           WHERE OrgCode = @orgCode   
               AND CreatedOn BETWEEN  @StartDate AND (SELECT DATEADD(day, 1, @EndDate))  
               AND ISDeleted<>1 ";

            // Fix: Use FirstOrDefault instead of ToList to return a single instance of VendorRequirementCount  
            return dbInstance.Select<VendorRequirementCount>(requirementQuery, new { request.OrgCode, request.StartDate, request.EndDate, request.UserId }).FirstOrDefault();
        }

        public async Task<List<dynamic>> GetCountTechStackByOrgCodeAsync(TechStackRequest request)
        {
            var dbInstance = GetDbInstance();

            var searchClause = string.IsNullOrWhiteSpace(request.SearchText)
                ? ""
                : "AND s.SkillName LIKE @searchText";

            var query = $@"
SELECT 
    s.SkillName,s.id,
    COUNT(DISTINCT r.Id) AS ResourceCount
FROM Skills s
INNER JOIN SkillResourcesMapping srm ON s.Id = srm.SkillId
INNER JOIN Resources r ON srm.ResourcesId = r.Id
WHERE r.OrgCode = @orgCode
{searchClause}
GROUP BY s.SkillName,s.Id
ORDER BY ResourceCount DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var parameters = new
            {
                orgCode = request.OrgCode,
                searchText = $"%{request.SearchText}%",
                offset = (request.Page - 1) * request.PageSize,
                pageSize = request.PageSize
            };

            return dbInstance.Select<dynamic>(query, parameters).ToList();
        }


        public async Task<List<Requirement>> GetPublicRequirementAsync(List<string> orgCode, int visibility)
        {
            var dbInstance = GetDbInstance();
            var sql = "select * from Requirement where  visibility=@visibility";

            //if(orgCode !=null && orgCode.Any())
            //{
            //    sql += "  and orgCode in @orgCode";
            //}
            var profile = dbInstance.Select<Requirement>(sql, new { orgCode,visibility }).ToList();
            return profile;
        }
        public async Task<List<Requirement>> GetPublicRequirementAsyncV2(string  orgCode, int visibility)
        {
            var dbInstance = GetDbInstance();
            var sql = "select * from Requirement where  visibility=@visibility and orgCode=@orgCode";

            var profile = dbInstance.Select<Requirement>(sql, new { orgCode, visibility }).ToList();
            return profile;
        }
        public async Task<List<Requirement>> GetRequirementsWithNoApplicantsAsync()
        {
            var dbInstance = GetDbInstance();
            var sql = @"
        SELECT *
        FROM Requirement
        WHERE Id NOT IN (
            SELECT RequirementId 
            FROM Applications 
            WHERE RequirementId IS NOT NULL
        )";

            var requirements = await dbInstance.SelectAsync<Requirement>(sql, new { });
            return requirements.ToList();
        }
    }
}
