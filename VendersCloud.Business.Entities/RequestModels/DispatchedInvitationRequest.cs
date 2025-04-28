namespace VendersCloud.Business.Entities.RequestModels
{
 
    public class DispatchedInvitationRequest
    {
        public string PartnerCode { get; set; }
        public string VendorCode { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

    }

  
}
