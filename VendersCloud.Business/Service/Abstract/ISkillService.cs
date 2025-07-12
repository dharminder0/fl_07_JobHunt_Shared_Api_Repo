namespace VendersCloud.Business.Service.Abstract
{
    public interface ISkillService
    {
        Task<List<Skills>> SkillUpsertAsync(List<string> skillnames);
        Task<List<string>> GetSkillListAsync();
    }
}
