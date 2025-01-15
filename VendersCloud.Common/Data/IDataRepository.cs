using DapperExtensions.Predicate;

namespace VendersCloud.Common.Data
{
    public interface IDataRepository<T> where T : class, new() {
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> Get();

        T Get(string id);

        IEnumerable<T> Get(int pageIndex, int pageSize, ref long totalItems);

        void Remove(string id);

        void Remove(T entity);

        T Update(T entity);
        void BulkInsert<E>(IEnumerable<E> data, string connectionName = null, List<string> excludedProperties = null);
        //T1 GetById<T1>(int? id, string connectionName = null);
        //T1 GetBy<T1>(IPredicate where, string connectionName = null);
        //T1 GetBy<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null);
        //int Count<T1>(int? id);
        int Count(IPredicate where, string connectionName = null);
        //bool Exists<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null);
        bool Exists(IPredicate where, string connectionName = null);
        //bool Exists<T1>(IPredicate where, string connectionName = null);
        //dynamic Insert<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null);
        //bool InsertWithTran<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null);
        //IEnumerable<dynamic> InsertAndGetIds<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null);
        T GetDapper(IPredicate pg, string connectionName = null);
        //T1 GetDapper<T1>(IPredicate pg, string connectionName = null);
        dynamic InsertDapper(T entity, string connectionName = null);
        IEnumerable<dynamic> InsertAndGetIds(IEnumerable<T> entities, string connectionName = null);
        void InsertDapper(IEnumerable<T> entities, string connectionName = null);
        bool UpdateDapper(T entity, string connectionName = null);
        bool DeleteBy(IPredicate where, string connectionName = null);
        //bool DeleteBy<T1>(IPredicate pg, string connectionName = null);
        bool DeleteDapper(T entity, string connectionName = null);
        bool DeleteDapper(int? id, string connectionName = null);
        //bool Delete<T1>(int id, string connectionName = null);
        //bool Delete<T1>(IEnumerable<int> ids, string connectionName = null);
        //bool DeleteWithTran<T1>(IEnumerable<int> ids, SqlConnection db, SqlTransaction tran, string connectionName = null);
        T GetByIdDapper(int? id, string connectionName = null);
        IEnumerable<T> GetAllDapper(string connectionName = null);
        //IEnumerable<T1> GetAllDapper<T1>(string connectionName = null);
        IEnumerable<T> GetListBy(IPredicate where, string connectionName = null);
        T GetBy(IPredicate where, string connectionName = null);
        //IEnumerable<T1> GetListBy<T1>(IPredicate where, string connectionName = null);
    }
}
