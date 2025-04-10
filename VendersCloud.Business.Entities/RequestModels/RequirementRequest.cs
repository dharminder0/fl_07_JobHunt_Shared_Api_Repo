namespace VendersCloud.Business.Entities.RequestModels
{
    public class RequirementRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Budget {  get; set; }
        public string Positions { get; set; }
        public string Duration { get; set; }
        public string LocationType { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public string OrgCode { get; set; }
        public List<string> Skills { get; set; }
        public int Status {  get; set; }
        public string ClientCode { get; set; }
        public string  UserId { get; set; }
    }
}
