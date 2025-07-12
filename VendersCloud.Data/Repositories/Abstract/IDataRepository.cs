namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IDataRepository<T> where T : class,new()
    {
        T Add(T entity, string[] excludedProperties = null);
        IEnumerable<T> Get();
        T Get(string id);
        IEnumerable<T> Get(int pageIndex, int pageSize, ref long totalItems);
        void Remove(string id);
        void Remove(T entity);
        T Update(T entity, string[] excludedProperties = null);
        void BulkInsert<E>(IEnumerable<E> data, string tableName, string connectionName = null, List<string> excludedProperties = null);
        T GetDapper(IPredicate pg, string connectionName = null);
        Task<T> GetDapperAsync(IPredicate pg, string connectionName = null);
        IEnumerable<T> GetAllDapper(string connectionName = null);
        Task<IEnumerable<T>> GetAllDapperAsync(string connectionName = null);
        IEnumerable<T> GetListBy(IPredicate where, string connectionName = null);
        Task<IEnumerable<T>> GetListByAsync(IPredicate where, string connectionName = null);
        T GetBy(IPredicate where, string connectionName = null);
        Task<T> GetByAsync(IPredicate where, string connectionName = null);
        bool Exists(IPredicate where, string connectionName = null);
        dynamic InsertDapper(T entity, string connectionName = null);
        Task<dynamic> InsertDapperAsync(T entity, string connectionName = null);
        IEnumerable<dynamic> InsertAndGetIds(IEnumerable<T> entities, string connectionName = null);
        bool UpdateDapper(T entity, string connectionName = null);
        Task<bool> UpdateDapperAsync(T entity, string connectionName = null);
        bool DeleteBy(IPredicate where, string connectionName = null);
        bool DeleteBy<T1>(IPredicate pg, string connectionName = null) where T1 : class, new();
        bool DeleteDapper(T entity, string connectionName = null);
        bool DeleteDapper(int? id, string connectionName = null);
        T GetByIdDapper(int? id, string connectionName = null);
        Task<T> GetByIdDapperAsync(int? id, string connectionName = null);
        int Execute(string sql, object param = null, string connectionName = null);
        IEnumerable<E> Query<E>(string query, object param = null, string connectionName = null);
        IEnumerable<E> ExecuteStoredProcedure<E>(string spName, object param = null, string connectionName = null);
        E QueryFirst<E>(string query, object param = null, string connectionName = null);
        string DecryptToken(string token);
    }
}
