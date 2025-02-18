using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Clients")]
    public class Clients : IEntityKey
    {
        public int Id { get; set; }
        public string ClientCode { get; set; }
        public string OrgCode { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string LogoURL { get; set; }
        public string FaviconURL { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ClientsMapper : ClassMapper<Clients>
    {
        public ClientsMapper()
        {
            Table("Clients");
            AutoMap();
        }
    }
}
