﻿namespace VendersCloud.Business.Entities.RequestModels
{
    public class SearchRequirementRequest
    {
        public string OrgCode { get; set; }
        public string SearchText {  get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public List<int> LocationType {  get; set; }
        public List<int> Status {  get; set; }
        public List<string> ClientCode { get; set; }
        public string UserId {  get; set; }
        public List<string> RoleType {  get; set; }

    }
}
