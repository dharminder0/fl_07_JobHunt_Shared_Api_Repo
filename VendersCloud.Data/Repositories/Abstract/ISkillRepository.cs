namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ISkillRepository : IBaseRepository<Skills>
    {
        Task<List<Skills>> SkillUpsertAsync(List<string> skillname);
    }
   
}
