namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Resources")]
    public class Resources :IEntityKey
    {
       public int Id { get; set; }
       public string FirstName {  get; set; }
       public string LastName { get; set; }
       public string Title { get; set; }
       public string Email { get; set; }
       public string CV { get; set; }
       public string OrgCode { get; set; }
       public int Availability { get; set; }
       public DateTime CreatedOn {  get; set; }
       public DateTime UpdatedOn { get; set; }
       public int CreatedBy { get; set; }
       public int UpdatedBy { get; set; }
       public bool IsDeleted { get; set; }
       public string Avtar { get; set; }
       public string SkillsEmbedding { get; set; }
    }

    public class ResourcesMapper : ClassMapper<Resources>
    {
        public ResourcesMapper() {
            Table("Resources");
            AutoMap();
        }
    }
}
