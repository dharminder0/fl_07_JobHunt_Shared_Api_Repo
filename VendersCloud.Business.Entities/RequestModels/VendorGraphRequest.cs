namespace VendersCloud.Business.Entities.RequestModels
{
    public class VendorGraphRequest
    {
        public string OrgCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }
    }
}
