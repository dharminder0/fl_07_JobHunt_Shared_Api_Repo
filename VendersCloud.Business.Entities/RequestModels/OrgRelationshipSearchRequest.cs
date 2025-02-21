namespace VendersCloud.Business.Entities.RequestModels
{
    public class OrgRelationshipSearchRequest
    {
        public string searchText { get; set; }
        public string OrgCode { get; set; }
        public List<string> RelationshipType { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
