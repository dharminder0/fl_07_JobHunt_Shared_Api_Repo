namespace VendersCloud.Business.Entities.ResponseModels
{
    public class ApplicantsSearchResponse
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Requirement {  get; set; }
        public int Status {  get; set; }
        public string StatusName {  get; set; }
        public DateTime ApplicationDate {  get; set; }
        public string CV { get; set; }
        public string ClientOrgLogo {  get; set; }
        public string ClientOrgName { get; set; }
    }
}
