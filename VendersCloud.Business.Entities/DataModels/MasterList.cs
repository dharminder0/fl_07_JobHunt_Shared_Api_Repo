namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "MasterList")]
    public class MasterList: IEntityKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class MasterListMapper : ClassMapper<MasterList>
    {
        public MasterListMapper() {
            Table("MasterList");
            AutoMap();
        }
    }
}
