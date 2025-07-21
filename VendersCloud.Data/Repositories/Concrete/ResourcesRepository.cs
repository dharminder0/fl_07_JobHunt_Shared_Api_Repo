using System.Linq;
using System.Numerics;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesRepository : StaticBaseRepository<Resources>, IResourcesRepository
    {
        private readonly IBenchRepository benchRepository;
        private readonly IRequirementRepository _requirementRepository;
        public ResourcesRepository(IConfiguration configuration, IBenchRepository _benchRepository,IRequirementRepository requirementRepository) : base(configuration)
        {
        benchRepository = _benchRepository;
            _requirementRepository = requirementRepository;
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
            foreach (var app in applicationsData)
            {
                var statusList = await benchRepository.GetStatusHistoryByApplicantId(app.Id);
                if (statusList != null && statusList.Any())
                {

                    app.Status = statusList.Select(v => v.Status).OrderByDescending(v => v).First();
                    app.Comment = statusList.Select(v => v.Comment).OrderByDescending(v => v).First();
                }


            }
            return applicationsData;
        }

        public async Task<List<dynamic>> GetApplicationsPerRequirementIdAsyncV2(int requirementId)
        {
            var dbInstance = GetDbInstance();

            var sql = @"
        SELECT a.ResourceId
        FROM Applications a
        JOIN (
            SELECT ApplicantId, Status
            FROM (
                SELECT ApplicantId, Status,
                       ROW_NUMBER() OVER (PARTITION BY ApplicantId ORDER BY ChangedOn DESC) AS rn
                FROM ApplicantStatusHistory
            ) ASH
            WHERE rn = 1
        ) latestStatus ON latestStatus.ApplicantId = a.Id
        WHERE a.RequirementId = @requirementId
          AND latestStatus.Status IN (8, 9, 10);";

            var applicationsData = (await dbInstance.SelectAsync<dynamic>(sql, new
            {
                requirementId
            })).ToList();

            return applicationsData;
        }
        public async Task<List<dynamic>> GetApplicationsPerRequirementIdAsyncV2(int requirementId, string vendorCode,int role)
        {
            var dbInstance = GetDbInstance();
            if (role == 1)
            {

                var sql = @"
        SELECT a.ResourceId
        FROM Applications a
        JOIN (
            SELECT ApplicantId, Status
            FROM (
                SELECT ApplicantId, Status,
                       ROW_NUMBER() OVER (PARTITION BY ApplicantId ORDER BY ChangedOn DESC) AS rn
                FROM ApplicantStatusHistory
            ) ASH
            WHERE rn = 1
        ) latestStatus ON latestStatus.ApplicantId = a.Id
        JOIN Users o ON a.CreatedBy = o.Id
        WHERE a.RequirementId = @requirementId
          AND latestStatus.Status IN (8, 9, 10)
          AND o.orgCode = @vendorCode;";

                var applicationsData = (await dbInstance.SelectAsync<dynamic>(sql, new
                {
                    requirementId,
                    vendorCode
                })).ToList();

                return applicationsData;
            }
            else
            {
                 var  data=await GetApplicationsPerRequirementIdAsyncV2(requirementId);
                return data;
            }
        }


        public async Task<List<Applications>> GetApplicationsPerRequirementIdAsync(int requirementId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Applications WHERE RequirementId = @requirementId";

            var applicationsData = dbInstance.Select<Applications>(sql, new { requirementId }).ToList();
            foreach (var app in applicationsData)
            {
                var statusList = await benchRepository.GetStatusHistoryByApplicantId(app.Id);
                if (statusList != null && statusList.Any())
                {
                    app.Status = statusList
                        .OrderByDescending(v => v.ChangedOn)
                        .Select(v => v.Status)
                        .FirstOrDefault();

                    app.Comment = statusList
                        .OrderByDescending(v => v.ChangedOn)
                        .Select(v => v.Comment)
                        .FirstOrDefault();
                }
            }


            return applicationsData;
        }


        public async Task<List<Applications>> GetApplicationsPerRequirementIdAsyncV2(List<int> requirementId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Applications WHERE RequirementId in @requirementId";

            var applicationsData = dbInstance.Select<Applications>(sql, new { requirementId }).ToList();
            foreach (var app in applicationsData)
            {
                var statusList = await benchRepository.GetStatusHistoryByApplicantId(app.Id);
                if (statusList != null && statusList.Any())
                {
                    app.Status = statusList
                        .OrderByDescending(v => v.ChangedOn)
                        .Select(v => v.Status)
                        .FirstOrDefault();

                    app.Comment = statusList
                        .OrderByDescending(v => v.ChangedOn)
                        .Select(v => v.Comment)
                        .FirstOrDefault();
                }
            }


            return applicationsData;
        }

        public async Task<int> GetTotalApplicationsPerRequirementIdAsync(int requirementId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT COUNT(ResourceId) FROM Applications WHERE RequirementId = @requirementId";

            var applicationsCount =  Query<int>(sql, new
            {
                requirementId
            }).FirstOrDefault();
            return applicationsCount;
        }

        public async Task<int> GetTotalApplicationsPerRequirementIdAsyncV2(int requirementId, string vendorCode, int role )
        {
            if (role == 1)
            {
                var dbInstance = GetDbInstance();
                var sql = @"
        SELECT COUNT(a.ResourceId)
        FROM Applications a
        INNER JOIN users o ON a.CreatedBy = o.Id
        WHERE a.RequirementId = @requirementId
          AND o.orgCode = @vendorCode";


                var applicationsCount = Query<int>(sql, new
                {
                    requirementId,
                    vendorCode
                }).FirstOrDefault();

                return applicationsCount;
            }
            else
            {
                int count =  await GetTotalApplicationsPerRequirementIdAsync(requirementId);
                return count;
            }
        }


        public async Task<int> GetTotalPlacementsAsync(List<int> requirementIds)
        {
            if (requirementIds == null || requirementIds.Count == 0)
                return 0;

            var db = GetDbInstance();

            var query = new Query("Applications AS a")
                .Join("ApplicantStatusHistory AS ash", "a.Id", "ash.ApplicantId")
                .WhereIn("a.RequirementId", requirementIds)
                .Where("ash.Status", 8)
                .SelectRaw("COUNT(DISTINCT a.ResourceId)");

            var result = await db.ExecuteScalarAsync<int>(query);
            return result;
        }


        public async Task<int> GetTotalPlacementsByUserIdsAsync(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                return 0;

            var dbInstance = GetDbInstance();

            var query = new Query("Applications")
                .Join("ApplicantStatusHistory AS ASH", "ASH.ApplicantId", "Applications.Id")
                .WhereIn("ASH.Status", new[] { 8, 9, 10 })
                .Where(q => q.WhereIn("Applications.CreatedBy", userIds).OrWhereIn("Applications.UpdatedBy", userIds))
                .SelectRaw("COUNT(DISTINCT Applications.ResourceId)");

            var result = await dbInstance.ExecuteScalarAsync<int>(query);
            return result;
        }


        public async Task<List<VendorDetailDto>> GetContractsByTypeAsync(VendorContractRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PartnerCode) || string.IsNullOrWhiteSpace(request.VendorCode))
                return new List<VendorDetailDto>();

            using var connection = GetConnection();

            var result = new List<VendorDetailDto>();
            var parameters = new DynamicParameters();
            parameters.Add("@orgcode", request.PartnerCode);
            parameters.Add("@vendorCode", request.VendorCode);

            if (request.IsOpenPosition)
            {
                // 1. Vendor's own open requirements
                var openVendorQuery = @"
SELECT 
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
r.id as RequirementId,
    CONCAT(res.FirstName, ' ', res.LastName) AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) FROM Applications a2 WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility,
    r.locationType
FROM RequirementVendors rv
INNER JOIN Requirement r ON rv.RequirementId = r.Id
LEFT JOIN Applications a ON a.RequirementId = r.Id
LEFT JOIN Resources res ON a.ResourceId = res.Id
WHERE rv.OrgCode = @vendorCode AND r.OrgCode = @orgcode
ORDER BY r.CreatedOn DESC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

                var vendorResults = await connection.QueryAsync<VendorDetailDto>(openVendorQuery, parameters);
                result.AddRange(vendorResults);

                // 2. Public requirements from the partner
                var publicQuery = @"
SELECT 
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    NULL AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) FROM Applications a2 WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility,
r.id as RequirementId
FROM Requirement r
WHERE r.Visibility = 3 AND r.OrgCode = @orgcode
ORDER BY r.CreatedOn DESC";

                var publicResults = await connection.QueryAsync<VendorDetailDto>(publicQuery, parameters);
                result.AddRange(publicResults);

                return result;
            }

            // For active or past contracts
            string statusFilter = "";
            if (request.IsActiveContracts)
                statusFilter = "ash.Status IN (9)";
            else if (request.IsPastContracts)
                statusFilter = "ash.Status IN (10)";

            if (!string.IsNullOrEmpty(statusFilter))
            {
                var contractQuery = $@"
SELECT 
    r.Title AS RequirementTitle,
    r.id as RequirementId,
    r.CreatedOn AS RequirmentPostedDate,
    CONCAT(res.FirstName, ' ', res.LastName) AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) FROM Applications a2 WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility,
 r.locationType
FROM Applications a
INNER JOIN Resources res ON a.ResourceId = res.Id
INNER JOIN Requirement r ON a.RequirementId = r.Id
INNER JOIN RequirementVendors rv ON rv.RequirementId = r.Id
CROSS APPLY (
    SELECT TOP 1 Status
    FROM ApplicantStatusHistory
    WHERE ApplicantId = a.Id
    ORDER BY ChangedOn DESC
) ash
WHERE {statusFilter}
  AND r.OrgCode = @orgcode
  AND rv.OrgCode = @vendorCode
ORDER BY a.CreatedOn DESC";


                var statusResults = await connection.QueryAsync<VendorDetailDto>(contractQuery, parameters);
                result.AddRange(statusResults);
            }

            return result;
        }





        public async Task<Dictionary<int, int>> GetPlacementsGroupedByRequirementAsync(List<int> requirementIds)
        {
            var dbInstance = GetDbInstance();
            var validStatuses = new List<int> { 8, 9, 10 };

            var query = new Query("Applications AS A")
                .Join("ApplicantStatusHistory AS H", "A.Id", "H.ApplicantId")
                .WhereIn("A.RequirementId", requirementIds)
                .WhereIn("H.Status", validStatuses)
                .GroupBy("A.RequirementId")
                .Select("A.RequirementId")
                .SelectRaw("COUNT(DISTINCT A.ResourceId) AS Total");

            var result = await dbInstance.GetAsync<(int RequirementId, int Total)>(query);

            return result.ToDictionary(x => x.RequirementId, x => x.Total);
        }

        public async Task<List<VendorDetailDto>> GetSharedContractsAsync(SharedContractsRequest request)
{
    using var connection = GetConnection();
    var parameters = new DynamicParameters();
    parameters.Add("@clientCode", request.ClientCode);

    var finalResults = new List<VendorDetailDto>();

    if (request.ContractType == (int)ContractType.Open)
    {
        const string openVendorQuery = @"
SELECT 
    r.Id AS RequirementId,
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility,
r.LocationType  as LocationType
FROM Requirement r

WHERE r.ClientCode = @clientCode
ORDER BY r.CreatedOn DESC; 
;
;
;";

        var vendorOpenResults = await connection.QueryAsync<VendorDetailDto>(openVendorQuery, parameters);
        finalResults.AddRange(vendorOpenResults);
    }
    else
    {
        int status = request.ContractType == (int)ContractType.Active ? 9 : 10;
        parameters.Add("@status", status);

                const string contractQuery = @"
SELECT TOP 10 
 ASH.status,
RS.FirstName, 
r.Id AS RequirementId,
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
	CONCAT(COALESCE(RS.FirstName, ''), ' ', COALESCE(RS.LastName, '')) AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    1 AS NumberOfApplicants, -- Since no COUNT, assuming 1 per application row
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility AS Visibility,
	r.UniqueId,
    o.OrgName AS VendorName,
    '' AS VendorLogo,
    o.OrgCode AS VendorCode,
    '' AS UniqueId,
    a.CreatedOn AS ContractStartDate,
    a.UpdatedOn AS ContractEndDate,
    '' AS LocationType,
    ash.status AS Status,
    RS.CreatedBy AS CreatedBy,
O.OrgCode AS OrgCode
  
FROM Requirement R
JOIN Applications A ON R.Id = A.RequirementId 
JOIN Resources RS ON A.ResourceId = RS.Id   
INNER JOIN RequirementVendors rv ON rv.RequirementId = r.Id
Join Organization O on RS.orgCode =O.orgCode
LEFT JOIN (
    SELECT ApplicantId, status
    FROM (
        SELECT ApplicantId, status, 
               ROW_NUMBER() OVER (PARTITION BY ApplicantId ORDER BY ChangedOn DESC) AS rn
        FROM ApplicantStatusHistory
    ) ASH
    WHERE rn = 1
) ASH ON ASH.ApplicantId = A.Id
WHERE R.ClientCode = @ClientCode 
and ASH.Status =@Status ";

                var results = await connection.QueryAsync<VendorDetailDto>(contractQuery, parameters);
        finalResults.AddRange(results);
    }

    return finalResults
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToList();
}




        public async Task<ApplicationDetailDto> GetApplicationWithVendorAndResourceByIdAsync(int applicationId)
        {
            using var connection = GetConnection();

  
            var sql = "SELECT * FROM Applications WHERE Id = @applicationId";
            var app = (await connection.QueryAsync<Applications>(sql, new { applicationId })).FirstOrDefault();

            if (app == null)
                return null;

       
            var resourceSql = "SELECT FirstName, LastName FROM Resources WHERE Id = @resourceId";
            var resource = (await connection.QueryAsync<Resources>(resourceSql, new { resourceId = app.ResourceId })).FirstOrDefault();
            string resourceName = resource != null ? $"{resource.FirstName} {resource.LastName}" : "N/A";

            var userSql = "SELECT OrgCode FROM Users WHERE Id = @userId";
            var vendorCode = await connection.ExecuteScalarAsync<string>(userSql, new { userId = app.CreatedBy }) ?? "N/A";

          
            var statusList = await benchRepository.GetStatusHistoryByApplicantId(app.Id);
            int latestStatus = app.Status;
            string latestComment = app.Comment;

            if (statusList != null && statusList.Any())
            {
                latestStatus = statusList
                    .OrderByDescending(v => v.ChangedOn)
                    .Select(v => v.Status)
                    .FirstOrDefault();

                latestComment = statusList
                    .OrderByDescending(v => v.ChangedOn)
                    .Select(v => v.Comment)
                    .FirstOrDefault();
            }


            var applicationDetail = new ApplicationDetailDto
            {
                Id = app.Id,
                RequirementId = app.RequirementId,
                ResourceId = app.ResourceId,
                ResourceName = resourceName,
                Status = latestStatus,
                Comment = latestComment,
                VendorCode = vendorCode,
                CreatedOn = app.CreatedOn,
                UpdatedOn = app.UpdatedOn
            };

            return applicationDetail;
        }











        public enum ContractType
        {
            Past = 1,
            Active = 2,
            Open = 3
        }

    }
}
