namespace VendersCloud.Business.Entities.RequestModels
{
    public class TotalApplicantsRequest
    {
        public string RequirementUniqueId {  get; set; }
        public int Status {  get; set; }
        public string  VendorCode { get; set; }
        public int Role { get; set; }
    }
}
