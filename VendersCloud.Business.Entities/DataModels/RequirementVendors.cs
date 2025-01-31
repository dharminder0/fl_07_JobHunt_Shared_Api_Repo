using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "RequirementVendors")]
    public class RequirementVendors :IEntityKey
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public string OrgCode { get; set; }
        public DateTime CreatedOn {  get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RequirementVendorsMapper : ClassMapper<RequirementVendors>
    {
        public RequirementVendorsMapper() {
            Table("RequirementVendors");
            AutoMap();
        }
    }
}
