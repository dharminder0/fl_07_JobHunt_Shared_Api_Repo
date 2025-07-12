namespace VendersCloud.Business.Entities.RequestModels
{
    public class ApplicationsRequest
    {
        public List<string> ResourceId {  get; set; }
        public string RequirementUniqueId { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
    }
}
