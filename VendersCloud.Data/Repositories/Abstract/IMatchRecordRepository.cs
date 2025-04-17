using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IMatchRecordRepository : IBaseRepository<MatchResults>
    {
        Task<List<dynamic>> GetMatchRecordByRequirementIdAsync(List<int> requirementIds, int matchscores);
        Task<List<dynamic>> GetMatchRecordByResourceIdAsync(List<int> resourceIds, int matchscores);
        Task<List<dynamic>> GetMatchRecordByResourceAndRequirementIdAsync(List<int> resourceIds, List<int> requirementIds, int matchscores);
        Task<int> GetMatchingCountByRequirementId(int requirementId);
    }
    
}
