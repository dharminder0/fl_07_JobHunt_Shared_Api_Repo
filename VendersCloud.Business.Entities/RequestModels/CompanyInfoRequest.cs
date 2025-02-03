namespace VendersCloud.Business.Entities.RequestModels
{
    public class CompanyInfoRequest
    {
        public int UserId { get; set; }
        public List<int> registrationType { get; set; }
        public string OrgName { get; set; }
        public string Portfolio { get; set; }
        public string ContactMail { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Strength { get; set; }
    }
}
