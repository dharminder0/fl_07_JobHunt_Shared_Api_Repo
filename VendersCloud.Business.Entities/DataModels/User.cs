using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name="[User]")]
    public class User :IEntityKey
    {
        [Key(AutoNumber=true)]
        public int Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}
