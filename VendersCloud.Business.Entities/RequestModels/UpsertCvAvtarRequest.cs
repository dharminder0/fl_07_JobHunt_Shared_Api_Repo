namespace VendersCloud.Business.Entities.RequestModels
{
    public class UpsertCvAvtarRequest
    {
        public int BenchId { get; set; }
        public List<FileRequest> LogoURL { get; set; }
    }
}
