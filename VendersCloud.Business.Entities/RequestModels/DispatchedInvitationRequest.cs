namespace VendersCloud.Business.Entities.RequestModels
{
    public class DispatchedInvitationBase
    {
        public string Email { get; set; }
        public string OrgCode { get; set; }
    }
    public class DispatchedInvitationRequest
    {
        public Dispatchers Sender { get; set; }
        public Recipient Receiver { get; set; }
        public string Message { get; set; }

    }

    public class Dispatchers : DispatchedInvitationBase
    {
        public int RoleType { get; set; }
    }
    public class Recipient: DispatchedInvitationBase
    {

    }
}
