﻿namespace VendersCloud.Business.Entities.ResponseModels
{
    public class BenchResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Linkedin { get; set; }
        public Dictionary<string, object> CV { get; set; }
        public string Avtar { get; set; }
        public string OrgCode { get; set; }
        public int Availability { get; set; }
        public string  AvailabilityName{get;set;}
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int MatchingCount { get; set; }
    }
}
