namespace VendersCloud.Business.Entities.Abstract
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public bool AutoNumber { get; set; }
    }
}
