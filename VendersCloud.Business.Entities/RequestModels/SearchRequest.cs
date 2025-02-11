namespace VendersCloud.Business.Entities.RequestModels
{
    public class SearchRequest
    {
        public string SearchText { get; set; } 
        public List<string> Technology { get; set; } 
        public List<string> Resource { get; set; }
        public List<string> Role { get; set; }
        public List<string> Strength { get; set; } 
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
