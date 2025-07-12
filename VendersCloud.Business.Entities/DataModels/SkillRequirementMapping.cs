namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "SkillRequirementMapping")]
    public class SkillRequirementMapping : IEntityKey
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public int RequirementId { get; set; }
    }

    public class SkillRequirementMappingMapper : ClassMapper<SkillRequirementMapping>
    {
        public SkillRequirementMappingMapper()
        {
            Table("SkillRequirementMapping");
            AutoMap();
        }
    }

}
