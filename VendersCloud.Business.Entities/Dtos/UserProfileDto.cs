namespace VendersCloud.Business.Entities.Dtos
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public string RoleName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
