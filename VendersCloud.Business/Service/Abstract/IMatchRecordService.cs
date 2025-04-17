namespace VendersCloud.Business.Service.Abstract
{
    public  interface IMatchRecordService
    {
        Task<List<dynamic>> GetMatchRecordAsync(MatchRecordRequest request);
    }
}
