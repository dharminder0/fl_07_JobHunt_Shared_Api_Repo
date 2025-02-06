namespace VendersCloud.Business.Entities.RequestModels
{
    public class OrganizationProfileRequest
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public int EmpCount { get; set; }
        public string Description { get; set; }
        public string RegAddress {  get; set; }
        public bool IsDeleted {  get; set; }
        public List<SocialProfile> Social { get; set; }
        public List<OfficeLocation> OrgLocation { get; set; }
    }

    public class SocialProfile
    {
        public string Platform { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class OfficeLocation
    {
        public string City { get; set; }
        public int State { get; set; }
    }
}
