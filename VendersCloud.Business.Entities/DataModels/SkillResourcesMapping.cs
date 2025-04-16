namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "SkillResourcesMapping")]
    public class SkillResourcesMapping : IEntityKey
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public int ResourceId { get; set; }

    }

    public class SkillResourcesMappingMapper : ClassMapper<SkillResourcesMapping>
    {
        public SkillResourcesMappingMapper()
        {
            Table("SkillResourcesMapping");
            AutoMap();
        }
    }
}
