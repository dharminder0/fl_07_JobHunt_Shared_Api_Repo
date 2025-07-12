namespace VendersCloud.Business.Entities.DataModels
{
    public interface IAudit
    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
