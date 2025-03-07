﻿namespace VendersCloud.Business.Entities.Dtos
{
    public class LoginResponseDto
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string OrgCode { get; set; }
        public List<string> Role { get; set; }
        public string CompanyIcon { get; set; }
        public string CompanyName { get; set; }
        public bool IsVerified { get; set; }
    }
}
