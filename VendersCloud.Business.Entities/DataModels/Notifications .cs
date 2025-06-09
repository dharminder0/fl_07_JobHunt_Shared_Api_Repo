namespace VendersCloud.Business.Entities.DataModels
{
    [Alias(Name = "Notifications")]
    public class Notifications : IEntityKey
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRead { get; set; }
        public string   Title { get; set; }
        public NotificationType NotificationType { get; set; }
    }
    public enum NotificationType
    {
        VendorEmpanelled = 1,
        ResourceStatusChanged = 2,
        ResourceApplied = 3
    }
    public class NotificationsMapper : ClassMapper<Notifications>
    {
        public NotificationsMapper()
        {
            Table("Notifications");
            AutoMap();
        }
    }
}
