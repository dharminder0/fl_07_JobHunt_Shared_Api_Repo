namespace VendersCloud.Business.Entities.Dtos
{
    public class RequirementDto
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
        public string Location { get; set; }
        public int ClientId { get; set; }
        public string Remarks { get; set; }
        public int Visibility { get; set; }
        public bool Hot { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
