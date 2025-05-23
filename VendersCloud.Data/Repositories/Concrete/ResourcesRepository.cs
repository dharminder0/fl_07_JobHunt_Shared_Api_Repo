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

        public async Task<List<dynamic>> GetApplicationsPerRequirementIdAsync(int requirementId, int status)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT ResourceId FROM Applications where RequirementId=@requirementId and Status=@status";

            var applicationsData = dbInstance.Select<dynamic>(sql, new
            {
                requirementId,status
            }).ToList();
            return applicationsData;
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

        public async Task<int> GetTotalPlacementsAsync(List<int> requirementIds)
        {
            if (requirementIds == null || requirementIds.Count == 0)
                return 0;

            var dbInstance = GetDbInstance();
            var query = new Query("Applications")
                .WhereIn("RequirementId", requirementIds)
                .Where("Status", 8)
                .SelectRaw("COUNT(DISTINCT ResourceId)");

            var result = await dbInstance.ExecuteScalarAsync<int>(query);
            return result;
        }

        public async Task<int> GetTotalPlacementsByUserIdsAsync(List<int> userId)
        {
            if (userId == null || userId.Count == 0)
                return 0;

            var dbInstance = GetDbInstance();
            var query = new Query("Applications")
                .Where("Status", 8)
                .Where(q => q.WhereIn("CreatedBy", userId).OrWhereIn("UpdatedBy", userId))
                .SelectRaw("COUNT(ResourceId)");

            var result = await dbInstance.ExecuteScalarAsync<int>(query);
            return result;
        }

        public async Task<List<VendorDetailDto>> GetContractsByTypeAsync(VendorContractRequest request)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();

            if (request.IsOpenPosition)
            {
                parameters.Add("@orgcode", request.PartnerCode);

                // Get partner's vendor code
                var vendorCodeQuery = "SELECT VendorCode FROM PartnerVendorRel WHERE PartnerCode = @orgcode";
                var vendorCode = await connection.QueryFirstOrDefaultAsync<string>(vendorCodeQuery, parameters);

                // Fetch from RequirementVendors if vendorCode is available
                var vendorResults = new List<VendorDetailDto>();
                if (!string.IsNullOrEmpty(vendorCode))
                {
                    var openPositionQuery = @"
SELECT 
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    CONCAT(res.FirstName, ' ', res.LastName) AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) 
        FROM Applications a2 
        WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility
FROM RequirementVendors rv
INNER JOIN Requirement r ON rv.RequirementId = r.Id
LEFT JOIN Applications a ON a.RequirementId = r.Id
LEFT JOIN Resources res ON a.ResourceId = res.Id
WHERE rv.OrgCode = @vendorCode
ORDER BY r.CreatedOn DESC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

                    var openParams = new DynamicParameters();
                    openParams.Add("@vendorCode", vendorCode);
                    var openResults = await connection.QueryAsync<VendorDetailDto>(openPositionQuery, openParams);
                    vendorResults = openResults.ToList();
                }

                // Fetch public requirements for this partner
                var publicQuery = @"
SELECT 
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    NULL AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) 
        FROM Applications a2 
        WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility
FROM Requirement r
WHERE r.Visibility = 3 AND r.OrgCode = @orgcode
ORDER BY r.CreatedOn DESC";

                var publicResults = await connection.QueryAsync<VendorDetailDto>(publicQuery, parameters);

                // Merge both results and return
                return vendorResults.Concat(publicResults).ToList();
            }

            // Status filter for active/past contracts
            string statusFilterCondition = "";
            if (request.IsActiveContracts)
            {
                statusFilterCondition = "WHERE ash.Status IN (9)";
            }
            else if (request.IsPastContracts)
            {
                statusFilterCondition = "WHERE ash.Status IN (10)";
            }

            var contractQuery = $@"
SELECT 
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    CONCAT(res.FirstName, ' ', res.LastName) AS ResourceName,
    '' AS ClientLogoUrl,
    r.ClientCode AS ClientName,
    (
        SELECT COUNT(*) 
        FROM Applications a2 
        WHERE a2.RequirementId = r.Id
    ) AS NumberOfApplicants,
    r.Positions AS NumberOfPosition,
    r.Duration AS ContractPeriod,
    r.Visibility
FROM Applications a
INNER JOIN Resources res ON a.ResourceId = res.Id
INNER JOIN Requirement r ON a.RequirementId = r.Id
CROSS APPLY (
    SELECT TOP 1 Status
    FROM ApplicantStatusHistory
    WHERE ApplicantId = a.Id
    ORDER BY ChangedOn DESC
) ash
{statusFilterCondition} AND r.OrgCode = '{request.PartnerCode}'
ORDER BY a.CreatedOn DESC";

            var results = await connection.QueryAsync<VendorDetailDto>(contractQuery, parameters);
            return results.ToList();
        }



        public async Task<Dictionary<int, int>> GetPlacementsGroupedByRequirementAsync(List<int> requirementIds)
        {
            var dbInstance = GetDbInstance();

            var query = new Query("Applications") 
                .WhereIn("RequirementId", requirementIds)
                .Where("Status", 8)
                .GroupBy("RequirementId")
                .Select("RequirementId")
                .SelectRaw("COUNT(DISTINCT ResourceId) AS Total");

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
Join Organization O on RS.CreatedBy =O.Id
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













        public enum ContractType
        {
            Past = 1,
            Active = 2,
            Open = 3
        }

    }
}
