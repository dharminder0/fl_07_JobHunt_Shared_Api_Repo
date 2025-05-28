namespace VendersCloud.Business.Entities.RequestModels
{
    public  class CompanyRequirementSearchRequest
    {
        public List<string> Client { get; set; }
        public List<int> Status { get; set; }
        public string SearchText { get; set; }
        public string OrgCode { get; set; }
        public string  RequirmentUniqueId { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
 