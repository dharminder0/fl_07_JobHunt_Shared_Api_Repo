namespace VendersCloud.Business.Entities.RequestModels
{
    public class SetPasswordRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserToken { get; set; }
    }
}
