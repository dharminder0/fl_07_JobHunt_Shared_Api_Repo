namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Skills")]
    public class Skills : IEntityKey
    {
        public int Id { get; set; }
        public string SkillName { get; set; }
    }

    public class SkillsMapper : ClassMapper<Skills>
    {
        public SkillsMapper()
        {
            Table("Skills");
            AutoMap();
        }
    }
}
