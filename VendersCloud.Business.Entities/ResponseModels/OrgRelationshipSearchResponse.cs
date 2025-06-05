using VendersCloud.Business.Entities.DataModels;

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
        public string OrgName { get; set; }
        public string Description { get; set; }
        public int EmpCount { get; set; }
        public string Logo { get; set; }
        public List<string> Location { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class NotificationListResponse
    {
        public int Count { get; set; }
        public List<Notifications> Notifications { get; set; }
    }

}
