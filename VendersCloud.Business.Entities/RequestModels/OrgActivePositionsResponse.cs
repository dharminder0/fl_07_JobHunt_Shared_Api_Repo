namespace VendersCloud.Business.Entities.RequestModels
{
    public class OrgActivePositionsResponse
    {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientFavicon { get; set; }
        public int TotalPositions { get; set; }
       
    }
    public class ApplicantStatusHistoryResponse
    {
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }

        public string Comment { get; set; }

    }

}
