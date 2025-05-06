namespace VendersCloud.Business.Entities.ResponseModels
{
    public class CompanyRequirementResponse
    {
        public string Role {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ResourceId { get; set; }
        public int RequirementId {  get; set; }
        public string RequirementUniqueId { get; set; }
        public int Status {  get; set; }
        public string StatusName { get; set; }
        public string OrgName { get; set; }
        public string OrgLogo { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int Position { get; set; }
        public int Placed { get; set; }
        public int Applicants { get; set; }
        public string CV { get; set; }
        public string ClientCode {  get; set; }
        public string ClientName { get; set; }
        public string ClientLogo { get; set; }
        public string VendorOrgCode { get; set; }
        public string VendorOrgName { get; set; }
        public string VendorLogo { get; set; }
        public string Comment { get; set; }
        public int MatchingScore { get; set; }

    }
}
