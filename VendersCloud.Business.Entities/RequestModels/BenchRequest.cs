namespace VendersCloud.Business.Entities.RequestModels
{
    public class BenchRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string CV { get; set; }
        public int Availability { get; set; }
        public string OrgCode { get; set; }
        public string UserId { get; set; }
    }
}
