namespace VendersCloud.Business.Entities.Dtos
{
    public class PaginationDto<T>
    {
        public int Count { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<T> List { get; set; }
    }
}
