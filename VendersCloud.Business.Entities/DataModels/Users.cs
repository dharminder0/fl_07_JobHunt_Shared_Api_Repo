namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Users")]
    public class Users :IEntityKey
    {
        public int Id { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string OrgCode { get; set; }
        public string Token { get; set; }
        public string Password {  get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender {  get; set; }
        public bool IsVerified {  get; set; }
        public string ProfileAvatar {  get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastLoginTime {  get; set; }
        public bool IsDeleted { get; set; }
        public string VerificationToken { get; set; }  
        public DateTime DOB {  get; set; }
        public string Phone { get; set; }
    }

    public class UserMapper : ClassMapper<Users>
    {
        public UserMapper()
        {
            Table("users");
            AutoMap();
        }
    }
}
