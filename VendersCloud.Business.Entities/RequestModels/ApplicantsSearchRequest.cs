namespace VendersCloud.Business.Entities.RequestModels
{
    public class ApplicantsSearchRequest
    {
        public string SearchText { get; set; }
        public List<string> ClientOrgCode { get; set; }
        public List<int> Status { get; set; }
        public string UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class VendorContractRequest
    {
   
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsActiveContracts { get; set; }
        public bool IsPastContracts { get; set; }
        public bool IsOpenPosition { get; set; }
        public bool IsBenchStrength { get; set; }
        public string PartnerCode { get; set; }
        public string  VendorCode { get; set; }
    }
    public class SharedContractsRequest
    {
        public string ClientCode { get; set; }
        public string ContractType { get; set; } 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
