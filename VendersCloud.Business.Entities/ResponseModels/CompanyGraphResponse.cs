namespace VendersCloud.Business.Entities.ResponseModels
{
    public class CompanyGraphResponse
    {
        public string OrgCode { get; set; }
        public string WeekDay { get; set; }
        public int TotalPositions { get; set; }
        public int TotalPlacements { get; set; }
    }

    public class VendorRequirementCount
    {
        public int Open { get; set; }
        public int Onhold { get; set; }
        public int Closed { get; set; }
    }
    public class VendorPlacementSummary
    {
        public string OrgCode { get; set; }
        public string WeekDay { get; set; }
        public int TotalPositions { get; set; }
        public int TotalPlacements { get; set; }
    }
    public class TechStackResponse
    {
        public string SkillName { get; set; }
        public int Id { get; set; }
        public int ResourceCount { get; set; }
    }
    public class PaginationResponse
    {
        public List<TechStackResponse> Data { get; set; }
        public int TotalCount { get; set; }
    }


}
