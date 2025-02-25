namespace VendersCloud.Business.Entities.RequestModels
{
    public class SearchMemberRequest
    {
        public string SearchText { get; set; }
        public string OrgCode { get; set; }
        public List<string> Access { get; set; }
        public int Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
