namespace VendersCloud.Business.Entities.RequestModels
{
    public class CompanyInfoRequest
    {
        public string UserId { get; set; }
        public List<string> registrationType { get; set; }
        public string OrgName { get; set; }
        public string Portfolio { get; set; }
        public string ContactMail { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Strength { get; set; }
        public string RegAddress { get; set; }
    }
}
