namespace VendersCloud.Business.Entities.Dtos
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Website { get; set; }
        public int EmpCount { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public string RegAddress { get; set; }
        public bool IsDeleted { get; set; }
        public List<string> Location { get; set; }
        public List<string> State { get; set; }
    }
}
