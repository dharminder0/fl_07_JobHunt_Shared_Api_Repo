﻿namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "UserProfiles")]
    public class UserProfiles:IEntityKey
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public bool IsDeleted { get; set; }

        public object Select(Func<object, string> value)
        {
            throw new NotImplementedException();
        }

        public class UserProfilesMapper : ClassMapper<UserProfiles>
        {
            public UserProfilesMapper() {
                Table("UserProfiles");
                AutoMap();
            }
        }
    }
}
