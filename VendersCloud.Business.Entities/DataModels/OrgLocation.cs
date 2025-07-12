namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "OrgLocation")]
    public class OrgLocation :IEntityKey
    {
        public int Id { get; set; }
        public string OrgCode {  get; set; }
        public string City {  get; set; }
        public int State {  get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class OrgLocationMapper : ClassMapper<OrgLocation>
    {
        public OrgLocationMapper()
        {
            Table("OrgLocation");
            AutoMap();
        }
    }
}
