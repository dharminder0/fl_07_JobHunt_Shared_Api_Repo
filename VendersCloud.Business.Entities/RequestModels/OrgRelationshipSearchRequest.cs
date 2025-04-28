namespace VendersCloud.Business.Entities.RequestModels
{
    public class OrgRelationshipSearchRequest
    {
        public string searchText { get; set; }
        public string OrgCode { get; set; }
        public string RelatedOrgCode { get; set; }
        public List<string> RelationshipType { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class ManageRelationshipStatusRequest
    {
        public int PartnerVendorRelId { get; set; }
        public int StatusId { get; set; }
        public int UpdatedBy { get; set; }
    }

}
