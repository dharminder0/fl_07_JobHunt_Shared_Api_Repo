namespace VendersCloud.Business.Entities.RequestModels
{
    public class ClientsRequest
    {
        public string OrgCode { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string LogoURL { get; set; }
        public string FaviconURL { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}
