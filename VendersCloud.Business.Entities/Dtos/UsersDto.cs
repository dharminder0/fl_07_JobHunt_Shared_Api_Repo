namespace VendersCloud.Business.Entities.Dtos
{
    public class UsersDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string OrgCode { get; set; }
        public string Gender { get; set; }
        public bool IsVerified { get; set; }
        public string ProfileAvatar { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string ? LastLoginTime { get; set; }
        public List<string> Role { get; set; }
        public string DOB { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; }
    }
}
