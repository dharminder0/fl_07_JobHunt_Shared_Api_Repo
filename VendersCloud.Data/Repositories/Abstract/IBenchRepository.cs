namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IBenchRepository :IBaseRepository<Resources>
    {
        Task<int> UpsertBenchMembersAsync(BenchRequest request);
        Task<List<Resources>> GetBenchResponseListAsync(string orgCode);
        Task<List<Resources>> GetBenchListBySearchAsync(BenchSearchRequest request);
        Task<IEnumerable<Resources>> GetBenchResponseListByIdAsync(List<int> benchId);
        Task<IEnumerable<Resources>> GetBenchResponseByIdAsync(int benchId);
        Task<bool> UpsertAvtarbyIdAsync(int id, string avtar);
        Task<IEnumerable<string>> GetAvtarByIdAsync(int benchId);
        Task<bool> InsertApplicantStatusHistory(ApplicantStatusHistory model);
        Task<List<ApplicantStatusHistory>> GetStatusHistoryByApplicantId(int applicantId);

    }
}
