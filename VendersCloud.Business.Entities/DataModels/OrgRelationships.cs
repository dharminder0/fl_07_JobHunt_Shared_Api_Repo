namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "OrgRelationships")]
    public class OrgRelationships: IEntityKey
    {
        public int Id { get; set; }
        public string OrgCode {  get; set; }
        public string RelatedOrgCode {  get; set; }
        public string RelationshipType {  get; set; }
        public int Status {  get; set; }
        public int CreatedBy {  get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn {  get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

    }

    public class OrgRelationshipsMapper:ClassMapper<OrgRelationships>
    {
        public OrgRelationshipsMapper()
        {
            Table("OrgRelationships");
            AutoMap();
        }
    }
}
