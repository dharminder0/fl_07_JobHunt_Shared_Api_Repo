namespace VendersCloud.Business.Entities.RequestModels
{
    public class BenchSearchRequest
    {
        public string SearchText {  get; set; }
        public string OrgCode {  get; set; }
        public List<int> Availability { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
