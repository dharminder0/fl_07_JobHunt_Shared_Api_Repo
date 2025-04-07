namespace VendersCloud.Business.Entities.RequestModels
{
    public class GetHotRequirmentRequest
    {
        public string OrgCode { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
