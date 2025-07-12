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
        public string RegAddress { get; set; }
        public List<FileRequest> Logo { get; set; }
        public bool IsDeleted { get; set; }
        public List<SocialProfiles> SocialLinks { get; set; }
        public List<OfficeLocations> OfficeLocation { get; set; }
    }

    public class SocialProfiles
    {
        public string Platform { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class OfficeLocations
    {
        public string City { get; set; }
        public int State { get; set; }
    }
}
