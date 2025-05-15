using System.Linq;
using System.Numerics;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesRepository : StaticBaseRepository<Resources>, IResourcesRepository
    {
        private readonly IBenchRepository benchRepository;
        public ResourcesRepository(IConfiguration configuration, IBenchRepository _benchRepository) : base(configuration)
        {
        benchRepository = _benchRepository;
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
                        app.Status = statusList.Select(v => v.Status).OrderByDescending(v => v).First();
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

            // Handle IsOpenPosition
            if (request.IsOpenPosition)
            {
                parameters.Add("@orgcode", request.PartnerCode); 
                var vendorCodeQuery = "SELECT VendorCode FROM PartnerVendorRel WHERE partnerCode = @orgcode";
                var vendorCode = await connection.QueryFirstOrDefaultAsync<string>(vendorCodeQuery, parameters);

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
                    return openResults.ToList();
                }

                return new List<VendorDetailDto>(); // no vendor code found
            }

            // Status filter for active/past contracts
            string statusFilter = "";

            if (request.IsActiveContracts)
            {
                statusFilter = "IN (9)";
            }
            else if (request.IsPastContracts)
            {
                statusFilter = "IN (10)";
            }

            var statusQuery = $@"
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
WHERE a.Status {statusFilter}
ORDER BY a.CreatedOn DESC";

            var results = await connection.QueryAsync<VendorDetailDto>(statusQuery, parameters);
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

            // Step 1: Get OrgCode of the client
            var orgCode = await connection.QueryFirstOrDefaultAsync<string>(
                "SELECT OrgCode FROM Clients WHERE ClientCode = @clientCode", parameters);

            if (string.IsNullOrEmpty(orgCode))
                return new List<VendorDetailDto>();

            parameters.Add("@partnerCode", orgCode);

            // Step 2: Get vendor codes for the client's org
            var vendorCodes = await connection.QueryAsync<string>(
                "SELECT VendorCode FROM PartnerVendorRel WHERE IsDeleted = 0 AND PartnerCode = @partnerCode AND StatusId = 2", parameters);

            if (!vendorCodes.Any())
                return new List<VendorDetailDto>();

            // Step 3: Dynamically build vendor filter
            var vendorCodeParams = vendorCodes.Select((vc, i) => $"@vendorCode{i}").ToList();
            for (int i = 0; i < vendorCodes.Count(); i++)
                parameters.Add($"@vendorCode{i}", vendorCodes.ElementAt(i));

            // Step 4: ContractType filter
            string contractTypeClause = "";
            if (request.ContractType == (int)ContractType.Open)
                contractTypeClause = "AND a.Status = 1";
            else if (request.ContractType == (int)ContractType.Active)
                contractTypeClause = "AND a.Status = 9";
            else if (request.ContractType == (int)ContractType.Past)
                contractTypeClause = "AND a.Status = 10";

            // Step 5: Final query
            string query = $@"
SELECT 
    r.Id AS RequirementId,
    r.Title AS RequirementTitle,
    r.CreatedOn AS RequirmentPostedDate,
    r.Positions AS NumberOfPosition,
    r.Visibility,
    r.Duration AS ContractPeriod,
    r.OrgCode,
    a.Status,
    c.ClientName,
    c.LogoURL AS ClientLogoUrl,
    res.FirstName + ' ' + res.LastName AS ResourceName,
    o.OrgName AS VendorName,
    o.OrgCode AS VendorCode,
    o.Website,
    o.Logo AS VendorLogo,
    r.UniqueId,
     a.CreatedOn as ContractstartDate,
    a.UpdatedOn as  ContractEndDate,
    r.LocationType,
    '' AS CVLink,
    COUNT( a.Id) OVER(PARTITION BY r.Id) AS NumberOfApplicants
FROM PartnerVendorRel pv
INNER JOIN Organization o ON o.OrgCode = pv.VendorCode
INNER JOIN Requirement r ON r.OrgCode ='{orgCode}'
INNER JOIN Clients c ON c.OrgCode = r.OrgCode
INNER JOIN Applications a ON a.RequirementId = r.Id
LEFT JOIN Resources res ON a.ResourceId = res.Id
WHERE pv.VendorCode IN ({string.Join(",", vendorCodeParams)})
  {contractTypeClause}
ORDER BY r.CreatedOn DESC";

            var results = await connection.QueryAsync<VendorDetailDto>(query, parameters);
            return results.ToList();
        }




        public enum ContractType
        {
            Past = 1,
            Active = 2,
            Open = 3
        }

    }
}
