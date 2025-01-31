namespace VendersCloud.Business.Entities.DataModels
{
    public interface IEntityAudit
    {
        bool? IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        int? DeletedBy { get; set; }
        int? CreatedBy { get; set; }
        int? UpdatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
