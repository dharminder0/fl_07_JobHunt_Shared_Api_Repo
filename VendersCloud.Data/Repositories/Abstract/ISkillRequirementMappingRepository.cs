namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ISkillRequirementMappingRepository : IBaseRepository<SkillRequirementMapping>
    {
        Task<int> UpsertSkillRequirementMappingAsync(int skillId, int requirementId);
        Task<List<SkillRequirementMapping>> GetSkillRequirementMappingAsync(int requirementId);
        Task<List<int>> GetRequirementIdsBySkillMatchAsync(List<int> skillIds, int excludeId);
    }
}
