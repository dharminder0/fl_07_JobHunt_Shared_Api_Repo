namespace VendersCloud.Business.Entities.RequestModels
{
    public class OrgActivePositionsResponse
    {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientLogo { get; set; }
        public int TotalPositions { get; set; }
    }
}
