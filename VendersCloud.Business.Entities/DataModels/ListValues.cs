using DapperExtensions.Mapper;
using VendersCloud.Business.Entities.Abstract;

namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "ListValues")]
    public class ListValues :IEntityKey
    {
        public int Id { get; set; }
        public int MasterListId {  get; set; }
        public string Value {  get; set; }
        public int ParentId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ListValuesMapper:ClassMapper<ListValues>
    {
        public ListValuesMapper()
        {
            Table("ListValues");
            AutoMap();
        }
    }
}
