namespace VendersCloud.Business.Entities.RequestModels
{
    public class AddMemberRequest
    {
        public string OrgCode { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public List<string> Access {  get; set; }
        public int Status { get; set; }
    }
}
