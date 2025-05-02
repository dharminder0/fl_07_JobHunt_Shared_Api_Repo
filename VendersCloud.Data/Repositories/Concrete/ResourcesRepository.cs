using System.Numerics;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class ResourcesRepository : StaticBaseRepository<Resources>, IResourcesRepository
    {
        public ResourcesRepository(IConfiguration configuration): base(configuration)
        {
        
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
            var sql = "SELECT * FROM Applications where RequirementId=@requirementId";

            var applicationsData = dbInstance.Select<Applications>(sql, new
            {
                requirementId
            }).ToList();
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




    }
}
