using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Empanelment")]
    public class Empanelment : IEntityKey
    {
        public int Id { get; set; }
        public string ClientOrgCode { get; set; }
        public string VendorOrgCode { get; set; }
        public string InitiatedBy { get; set; }
        public int Status { get; set; }
        public int Createdby { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class EmpanelmentMapper:ClassMapper<Empanelment>
    { 
        public EmpanelmentMapper()
        {
            Table("Empanelment");
            AutoMap();
        }

    }
}
