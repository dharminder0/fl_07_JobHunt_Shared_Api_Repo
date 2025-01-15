using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name= "UserCompanyMapping")]
    public class UserCompanyMapping :IEntityKey
    {
        [Key(AutoNumber = true)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyCode { get; set; }
    }
}
