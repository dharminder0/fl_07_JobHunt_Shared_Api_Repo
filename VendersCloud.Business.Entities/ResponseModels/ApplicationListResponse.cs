namespace VendersCloud.Business.Entities.ResponseModels
{
    public class ApplicationListResponse
    {
        public int RequirementId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string VendorOrgCode { get; set; }
        public string VendorOrgName { get; set; }
        public string vendorLogo { get; set; }
        public string  CvData { get; set; }
        public int MatchingScore { get; set; }

    }
}
