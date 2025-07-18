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
        public string  UniqueId { get; set; }
        public string  OrgCOde { get; set; }
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
        public int ContractType { get; set; } 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class TechStackRequest
    {
        public string  OrgCode { get; set; }
        public string  SearchText { get; set; }
        public int   PageSize { get; set; }
        public int Page { get; set; }
    }
    public class MatchingRequirementRequest
    {
        public long RequirementId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrgCode { get; set; }
    }

    public class MatchingRequirementDto
    {
        public long Id { get; set; }      // Requirement.Id
        public string UniqueId { get; set; }      // e.g. Req‑2025‑0001
        public string Title { get; set; }
        public int MatchingSkillCount { get; set; }      // shared skills with source requirement
        public int MatchingCandidateCount { get; set; }      // distinct resources whose skills match this requirement
        public string PartnerName { get; set; }
        public string PartnerLogo { get; set; }
        public string PartnerCode { get; set; }
    }

}
