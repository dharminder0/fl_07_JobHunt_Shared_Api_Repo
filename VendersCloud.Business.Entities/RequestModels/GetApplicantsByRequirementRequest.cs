namespace VendersCloud.Business.Entities.RequestModels
{
    public class GetApplicantsByRequirementRequest
    {
        public string SearchText { get; set; }
        public string RequirementUniqueId { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
