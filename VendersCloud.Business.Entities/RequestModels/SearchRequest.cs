namespace VendersCloud.Business.Entities.RequestModels
{
    public class SearchRequest
    {
        public string SearchText { get; set; } = "";
        public string Technology { get; set; } = "";
        public int? Resource { get; set; } = null;
        public List<string> Role { get; set; }
        public int? Strength { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
