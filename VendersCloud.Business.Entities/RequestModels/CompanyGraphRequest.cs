namespace VendersCloud.Business.Entities.RequestModels
{
    public class CompanyGraphRequest
    {
        public string OrgCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
