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
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime DOB { get; set; }
        public bool IsDeleted { get; set; }
    }
}
