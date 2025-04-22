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
    }
}
