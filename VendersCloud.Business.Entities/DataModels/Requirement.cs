namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Requirement")]
    public class Requirement : IEntityKey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string OrgCode { get; set; }
        public string Description {  get; set; }
        public string Experience {  get; set; }
        public string Budget {  get; set; }
        public int Positions { get; set; }
        public string Duration {  get; set; }
        public int LocationType { get; set; }
        public string Location { get; set; }
        public string ClientCode {  get; set; }
        public string Remarks { get; set; }
        public int Visibility {  get; set; }
        public bool Hot { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy {  get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted {  get; set; }
        public string UniqueId { get; set; }
        public string Embedding { get; set; }

    }

    public class RequirementMapper:ClassMapper<Requirement>
    {
        public RequirementMapper()
        {
            Table("Requirement");
            AutoMap();
        }
    }
}
