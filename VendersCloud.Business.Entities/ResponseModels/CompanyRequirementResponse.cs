namespace VendersCloud.Business.Entities.ResponseModels
{
    public class CompanyRequirementResponse
    {
        public string Role {  get; set; }
        public int RequirementId {  get; set; }
        public int Status {  get; set; }
        public string StatusName { get; set; }
        public DateTime DatePosted { get; set; }
        public int Position { get; set; }
        public int Placed { get; set; }
        public int Applicants { get; set; }
        public string ClientCode {  get; set; }
        public string ClientName { get; set; }
        public string ClientLogo { get; set; }
        public int Resources { get; set; }
        public string ResourcesName { get; set; }
    }
}
