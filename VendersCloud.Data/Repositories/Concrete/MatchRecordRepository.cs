namespace VendersCloud.Data.Repositories.Concrete
{
    public class MatchRecordRepository : StaticBaseRepository<MatchResults> , IMatchRecordRepository
    {
        public MatchRecordRepository( IConfiguration configuration) : base(configuration)
        {
        }
        
        public async Task<List<dynamic>> GetMatchRecordByRequirementIdAsync(List<int> requirementIds,int matchscores)
        {
            var dbInstance = GetDbInstance();
            var sql = @"SELECT
                COUNT(ResourceId) AS TotalCandidates,
                RequirementId,
                MatchScore , 
                STRING_AGG(ResourceId, ',') AS ResourceIds
            FROM MatchResults
            WHERE RequirementId IN @Ids AND MatchScore >= @matchscores
            GROUP BY RequirementId, MatchScore";
            var namedata = await dbInstance.SelectAsync<dynamic>(sql, new { Ids = requirementIds,matchscores });
            return namedata.ToList();
        }

        public async Task<List<dynamic>> GetMatchRecordByResourceIdAsync(List<int> resourceIds, int matchscores)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT Count(RequirementId) As MatchingRequirements,STRING_AGG(RequirementId, ',') AS  RequirementIds,ResourceId, MatchScore FROM MatchResults WHERE ResourceId IN @Ids And MatchScore >= @matchscores Group By RequirementId,ResourceId, MatchScore";
            var namedata = await dbInstance.SelectAsync<dynamic>(sql, new { Ids = resourceIds, matchscores });
            return namedata.ToList();
        }

        public async Task<List<dynamic>> GetMatchRecordByResourceIdAsync(int resourceIds)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM MatchResults WHERE ResourceId = @Ids ";
            var namedata = await dbInstance.SelectAsync<dynamic>(sql, new { Ids = resourceIds });
            return namedata.ToList();
        }
        public async Task<List<dynamic>> GetMatchRecordByResourceAndRequirementIdAsync(List<int> resourceIds, List<int> requirementIds,int matchscores)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT ResourceId,RequirementId, MatchScore FROM MatchResults WHERE ResourceId IN @ResourceIds AND RequirementId IN @RequirementIds And MatchScore >= @matchscores Group By RequirementId,ResourceId, MatchScore";
            var namedata = await dbInstance.SelectAsync<dynamic>(sql, new { ResourceIds = resourceIds, RequirementIds = requirementIds, matchscores });
            return namedata.ToList();
        }

        public async Task<List<int>> GetMatchingCountByRequirementId(int requirementId)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT ResourceId As MatchingCandidate FROM MatchResults WHERE RequirementId = @Ids ";
            var namedata = await dbInstance.SelectAsync<int>(sql, new { Ids = requirementId });
            var namedatas = namedata.ToList();
            return namedatas;
        }

        public async Task<dynamic> GetMatchScoreAsync(int requirementId,int resourceId)
        {
            var dbInstance = GetDbInstance();
            var sql = "Select MatchScore From MatchResults Where RequirementId= @requirementId And ResourceId= @resourceId";
            var namedata = await dbInstance.SelectAsync<dynamic>(sql, new { requirementId, resourceId });
            var namedatas = namedata.FirstOrDefault();
            return namedatas;
        }
}}
