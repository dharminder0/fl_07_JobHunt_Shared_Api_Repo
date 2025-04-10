namespace VendersCloud.Business.Entities.ResponseModels
{
    public class RequirementResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string OrgCode { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Budget { get; set; }
        public int Positions { get; set; }
        public string Duration { get; set; }
        public int LocationType { get; set; }
        public string LocationTypeName { get; set; }
        public string Location { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientLogo { get; set; }
        public string Remarks { get; set; }
        public int Visibility { get; set; }
        public string VisibilityName { get; set; }
        public int Placed {  get; set; }
        public int Applicants { get; set; }
        public bool Hot { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string UniqueId { get; set; }
        public List<string> Skills { get; set; }
    }
  
}
