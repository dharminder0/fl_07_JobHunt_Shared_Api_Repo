namespace VendersCloud.Business.Entities.RequestModels
{
    public class CompanyInfoRequest
    {
        public int UserId { get; set; }
        public int reqistrationType { get; set; }
        public string CompanyName { get; set; }
        public string Portfolio { get; set; }
        public string ContactMail { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Strength { get; set; }
    }
}
