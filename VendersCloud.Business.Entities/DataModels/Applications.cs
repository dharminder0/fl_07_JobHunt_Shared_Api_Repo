namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Applications")]
    public class Applications : IEntityKey
    {
        public int Id { get; set; }
        public int ResourceId {  get; set; }
        public int RequirementId {  get; set; }
        public int Status {  get; set; }
        public string Comment { get; set; }
        public DateTime CreatedOn {  get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy {  get; set; }
        public int UpdatedBy { get; set; }
    }

    public class ApplicationsMapper : ClassMapper<Applications>
    {
        public ApplicationsMapper()
        {
            Table("Applications");
            AutoMap();
        }
    }
}
