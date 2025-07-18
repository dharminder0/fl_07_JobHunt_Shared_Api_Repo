﻿namespace VendersCloud.Data.Repositories.Abstract
{
    public interface ISkillRepository : IBaseRepository<Skills>
    {
        Task<List<Skills>> SkillUpsertAsync(List<string> skillname);
        Task<List<string>> GetAllSkillNamesAsync(List<int> skillIds);
        Task<List<string>> GetAllSkillNamesAsync();
    }
   
}
