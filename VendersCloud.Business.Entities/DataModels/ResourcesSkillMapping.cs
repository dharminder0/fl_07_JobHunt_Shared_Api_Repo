namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "ResourcesSkillMapping")]
    public class ResourcesSkillMapping : IEntityKey
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public int ResourceId { get; set; }

    }

    public class ResourcesSkillMappingMapper : ClassMapper<ResourcesSkillMapping>
    {
        public ResourcesSkillMappingMapper()
        {
            Table("ResourcesSkillMapping");
            AutoMap();
        }
    }
}
