using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "OrgSocial")]
    public class OrgSocial : IEntityKey
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string Platform { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public bool isDeleted {  get; set; }

    }
    public class OrgSocialMapper : ClassMapper<OrgSocial>
    {
        public OrgSocialMapper() {
            Table("OrgSocial");
            AutoMap();
        }
    }
}
