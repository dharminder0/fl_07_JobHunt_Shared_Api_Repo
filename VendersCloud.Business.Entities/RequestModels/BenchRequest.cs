namespace VendersCloud.Business.Entities.RequestModels
{
    public class BenchRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public Cv cv { get; set; }
        public int Availability { get; set; }
        public string OrgCode { get; set; }
        public string UserId { get; set; }
    }
    public class ContactDetails
    {
        public string email { get; set; }
        public string phone { get; set; }
        public string linkedin { get; set; }
    }

    public class Cv
    {
        public Profile profile { get; set; }
        public List<string> summary { get; set; }
        public List<Project> projects { get; set; }
        public ContactDetails contact_details { get; set; }
        public List<string> certifications { get; set; }
        public List<string> top_skills { get; set; }
        public List<string> education { get; set; }
    }

    public class Profile
    {
        public string name { get; set; }
        public string title { get; set; }
        public string experience { get; set; }
        public string objective { get; set; }
    }

    public class Project
    {
        public string title { get; set; }
        public string role { get; set; }
        public string description { get; set; }
        public List<string> responsibilities { get; set; }
    }
}
