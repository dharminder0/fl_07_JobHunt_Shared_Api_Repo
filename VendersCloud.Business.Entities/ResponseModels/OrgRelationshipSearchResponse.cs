namespace VendersCloud.Business.Entities.ResponseModels
{
    public class OrgRelationshipSearchResponse
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string RelatedOrgCode { get; set; }
        public string RelationshipType { get; set; }
        public string StatusName { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
