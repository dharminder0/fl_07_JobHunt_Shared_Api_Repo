namespace VendersCloud.Business.Entities.ResponseModels
{
    public class UserLoginResponseModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string CompanyIcon { get; set; }
        public string CompanyName { get; set; }
    }
}
