namespace VendersCloud.Business.Entities.Abstract
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property)]
    public class AliasAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
