namespace VendersCloud.Business.Entities.RequestModels
{
    public class UpdateUserProfileRequest
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; } 
    }
}
