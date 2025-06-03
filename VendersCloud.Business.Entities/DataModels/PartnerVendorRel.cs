namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "PartnerVendorRel")]
    public class PartnerVendorRel : IEntityKey
    {
        public int Id { get; set; }
        public string PartnerCode { get; set; }
        public string VendorCode { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string  Message { get; set; }
    }

    public class PartnerVendorRelMapper : ClassMapper<PartnerVendorRel>
    {
        public PartnerVendorRelMapper()
        {
            Table("PartnerVendorRel");
            AutoMap();
        }
    }
}
