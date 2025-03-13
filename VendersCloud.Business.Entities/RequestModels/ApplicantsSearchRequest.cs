namespace VendersCloud.Business.Entities.RequestModels
{
    public class ApplicantsSearchRequest
    {
        public string SearchText { get; set; }
        public string ClientOrgName { get; set; }
        public int Status { get; set; }
        public string UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
