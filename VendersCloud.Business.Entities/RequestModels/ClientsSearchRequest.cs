namespace VendersCloud.Business.Entities.RequestModels
{
    public class ClientsSearchRequest
    {
        public string searchText { get; set; }
        public string OrgCode { get; set; }
        public string Status { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
