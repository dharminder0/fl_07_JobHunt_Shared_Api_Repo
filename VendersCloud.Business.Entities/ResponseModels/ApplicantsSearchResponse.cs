﻿namespace VendersCloud.Business.Entities.ResponseModels
{
    public class ApplicantsSearchResponse
    {
        public int Id { get; set; }
        public string UniqueId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Title {  get; set; }
        public int Status {  get; set; }
        public string StatusName {  get; set; }
        public DateTime ApplicationDate {  get; set; }
        public Dictionary<string, object> CV { get; set; }
        public string OrgLogo {  get; set; }
        //public string  Avatar { get; set; }
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public string Comment { get; set; }
        public int MatchScore { get; set; }
        public int ApplicationId { get; set; }
    }
    public class VendorContractResponse
    {
        public string UserId { get; set; }
        public int TotalRecords { get; set; }
        public List<VendorDetailDto> Records { get; set; }
    }
    public class VendorContractResponseV2
    {
        public string UserId { get; set; }
        public int TotalRecords { get; set; }
        public List<VendorDetailDtoV2> Records { get; set; }
    }
    public class VendorDetailDto
    {
        public string  OrgCode { get; set; }
        public int RequirementId { get; set; }
        public string RequirementTitle { get; set; }
        public DateTime RequirmentPostedDate { get; set; }
        public string ResourceName { get; set; }
        public string ClientLogoUrl { get; set; }
        public string ClientName { get; set; }
        public int NumberOfApplicants { get; set; }
        public int NumberOfPosition { get; set; }
        public string  ContractPeriod  { get; set; }
        public string  Visibility { get; set; }
        public string CVLink { get; set; }
        public string  VendorName { get; set; }
        public string  VendorLogo { get; set; }
        public string  VendorCode { get; set; }
        public string UniqueId { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public string  LocationType { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
    }
    public class VendorDetailDtoV2
    {
        public string RequirementTitle { get; set; }
        public DateTime RequirmentPostedDate { get; set; }
        public int NumberOfPosition { get; set; }
        public string Visibility { get; set; }
        public string ContractPeriod { get; set; }
        public string CVLink { get; set; }
        public int status { get; set; }


        public string ClientName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Website { get; set; }
        public string LogoURL { get; set; }
    }

    public class ApplicationDetailDto
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public string VendorCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
