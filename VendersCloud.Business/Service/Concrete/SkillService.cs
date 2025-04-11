using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Concrete
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        public SkillService(ISkillRepository skillRepository) {
            _skillRepository = skillRepository;
        }

        public async Task<List<Skills>> SkillUpsertAsync(List<string> skillnames)
        {
            try
            {
                var skill = await _skillRepository.SkillUpsertAsync(skillnames);
                return skill;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while upserting skills", ex);
            }
        
        }

        public async Task<List<string>> GetSkillListAsync()
        {
            try
            {
                var skill = await _skillRepository.GetAllSkillNamesAsync();
                return skill;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting skills", ex);
            }
        }
    }
}
