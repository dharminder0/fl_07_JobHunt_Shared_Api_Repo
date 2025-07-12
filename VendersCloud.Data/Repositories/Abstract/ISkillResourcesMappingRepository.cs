using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ISkillResourcesMappingRepository : IBaseRepository<SkillResourcesMapping>
    {
        Task<int> UpsertSkillRequirementMappingAsync(int skillId, int requirementId);
    }
}
