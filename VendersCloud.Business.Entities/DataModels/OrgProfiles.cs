using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "OrgProfiles")]
    public class OrgProfiles :IEntityKey
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public int ProfileId {  get; set; }
        public bool IsDeleted { get; set; }
    }

    public class OrgProfilesMapper : ClassMapper<OrgProfiles>
    {
        public OrgProfilesMapper() {
            Table("OrgProfiles");
            AutoMap();
        }
    }
}
