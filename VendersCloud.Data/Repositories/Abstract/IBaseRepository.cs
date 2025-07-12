namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IBaseRepository<T> : IDataRepository<T> where T : class, new()
    {
        T Upsert(T entity);
        Task<T> UpsertAsync(T entity);
        T1 UpsertWithTran<T1>(T1 entity, SqlConnection db, SqlTransaction tran, int? userId = null) where T1 : class, IEntityKey, new();
        bool Delete<T1>(int id, string connectionName = null) where T1 : class, IEntityKey, new();
        bool DeleteWithTran<T1>(IEnumerable<int> ids, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new();
        bool DeleteWithTran<T1>(IPredicate predicate, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new();
        bool ClearCacheKeysByText(string text);
        bool ClearAllClientCache(string clientCode);
    }
}
