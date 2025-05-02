namespace VendersCloud.Business.Entities.ResponseModels
{
    public class ApplicantsSearchResponse
    {
        public int Id { get; set; }
        public string UniqueId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Title {  get; set; }
        public int Status {  get; set; }
        public string StatusName {  get; set; }
        public DateTime ApplicationDate {  get; set; }
        public Dictionary<string, object> CV { get; set; }
        public string ClientOrgLogo {  get; set; }
        public string ClientOrgName { get; set; }
        public string ClientCode { get; set; }
        public string Comment { get; set; }
        public int MatchScore { get; set; }
    }
    public class VendorContractResponse
    {
        public string UserId { get; set; }
        public int TotalRecords { get; set; }
        public List<VendorDetailDto> Records { get; set; }
    }
    public class VendorDetailDto
    {
        public string RequirementTitle { get; set; }
        public DateTime RequirmentPostedDate { get; set; }
        public string ResourceName { get; set; }
        public string ClientLogoUrl { get; set; }
        public string ClientName { get; set; }
        public int NumberOfApplicants { get; set; }
        public int NumberOfPosition { get; set; }
        public string  ContractPeriod  { get; set; }
        public string  Visibility { get; set; }
    }

}
