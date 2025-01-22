namespace VendersCloud.Business.Entities.DTOModels
{
    public  class UserDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string UserId { get; set; }
        public string RoleType { get; set; }
    }
}
