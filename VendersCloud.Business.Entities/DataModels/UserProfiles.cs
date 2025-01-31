using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "UserProfiles")]
    public class UserProfiles:IEntityKey
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public bool IsDeleted { get; set; }

        public class UserProfilesMapper : ClassMapper<UserProfiles>
        {
            public UserProfilesMapper() {
                Table("UserProfiles");
                AutoMap();
            }
        }
    }
}
