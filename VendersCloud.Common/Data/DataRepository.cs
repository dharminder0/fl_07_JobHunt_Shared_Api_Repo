using VendersCloud.Common.Configuration;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using Newtonsoft.Json.Linq;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using VendersCloud.Common.Settings;
using VendersCloud.Business.Entities.Abstract;
using KeyAttribute = VendersCloud.Business.Entities.Abstract.KeyAttribute;

namespace VendersCloud.Common.Data
{

    public class DataRepository<T> where T : class, new()
    {

        protected SqlConnection db;
        protected string _connectionName;
        private SqlServerCompiler _sqlServerCompiler = new SqlServerCompiler();

        public DataRepository(string connectionName)
        {
            _connectionName = connectionName;
        }

        public DataRepository()
        {
            //_connectionName = ConfigurationManager.AppSettings["DefaultConnectionName"];
            _connectionName = GlobalSettings.DefaultConnectionName;
        }

        protected SqlConnection GetConnection(string connectionName = null)
        {
            try { return new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName ?? _connectionName].ConnectionString); }
            catch (Exception ex)
            {
                return GetConnectionv2(connectionName);
            }
        }
        protected SqlConnection GetConnectionv2(string connectionName = null)
        {
            connectionName = connectionName ?? _connectionName;
            var filePath = GlobalSettings.FilePath;
            var connectionString = File.Exists(filePath)
                                       ? JObject.Parse(File.ReadAllText(filePath))["ConnectionStrings"]?[connectionName]?.ToString()
                                       : null;

            if (connectionString == null)
            {
                throw new KeyNotFoundException($"The given key '{connectionName}' was not present in the configuration or JSON file.");
            }

            return new SqlConnection(connectionString);
        }
        //public QueryFactory GetDbInstance(string connectionName = null) {
        //    var connection = GetConnection(connectionName);
        //    var qFactory = new QueryFactory(connection, _sqlServerCompiler);
        //    qFactory.QueryTimeout = 5000000;
        //    return qFactory;
        //}

        public QueryFactory GetDbInstance(string connectionName = null)
        {
            var connection = GetConnection(connectionName);
            var qFactory = new QueryFactory(connection, _sqlServerCompiler);
            qFactory.Logger = compiled => {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine(compiled.ToString());
                Console.WriteLine("--------------------------------------");
                Debug.WriteLine("--------------------------------------");
                Debug.WriteLine(compiled.ToString());
                Debug.WriteLine("--------------------------------------");
            };
            qFactory.QueryTimeout = 5000000;
            return qFactory;
        }

        public string CompileQuery(Query query)
        {
            return _sqlServerCompiler.Compile(query).Sql;
        }

        public T Add(T entity)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat(@"
DECLARE @EntityId BIGINT
INSERT INTO [{0}](", GetAliasName());
            var isKeyAutoNumber = GetKeyAttribute().AutoNumber;
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat("[{0}],", GetKeyProperty().Name);
            }
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("[{0}],", prop.Name);
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(") VALUES(");
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat("@{0},", GetKeyProperty().Name);
            }
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("@{0},", prop.Name);
            }
            sql.Remove(sql.Length - 1, 1);
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat(@"); 
SELECT *
FROM {0}
WHERE {1} = '{2}'
", GetAliasName(), GetKeyProperty().Name, GetKeyProperty().GetValue(entity));
            }
            else
            {
                sql.AppendFormat(@"); 
SET @EntityId = SCOPE_IDENTITY();
SELECT *
FROM {0}
WHERE {1} = @EntityId
", GetAliasName(), GetKeyProperty().Name);
            }

            using (db = GetConnection())
            {
                entity = db.QueryFirstOrDefault<T>(sql.ToString(), entity);
                return entity;
            }

        }

        public IEnumerable<T> Get()
        {
            using (db = GetConnection())
            {
                return db.Query<T>("SELECT * FROM " + GetAliasName());
            }
        }

        public T Get(string id)
        {
            using (db = GetConnection())
            {
                return db.Query<T>(string.Format("SELECT * FROM [{0}] WHERE [{1}] = @id;", GetAliasName(), GetKeyProperty().Name), new { id = id }).FirstOrDefault();
            }
        }

        public IEnumerable<T> Get(int pageIndex, int pageSize, ref long totalItems)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
WITH CTE
AS
(
SELECT *,ROW_NUMBER() OVER(ORDER BY {1}) RowNum
FROM [{0}]
)
SELECT *
FROM CTE
WHERE RowNum BETWEEN @Start AND @End;", GetAliasName(), GetKeyProperty().Name);
            using (db = GetConnection())
            {
                return db.Query<T>(sql.ToString(), new { Start = pageIndex - 1, End = ((pageIndex - 1) * pageSize) + pageSize });
            }
        }

        public void Remove(string id)
        {
            using (db = GetConnection())
            {
                db.Execute(string.Format("DELETE FROM [{0}] WHERE [{1}] = @id;", GetAliasName(), GetKeyProperty().Name), new { id = id });

            }
        }

        public void Remove(T entity)
        {
            using (db = GetConnection())
            {
                db.Execute(string.Format("DELETE FROM [{0}] WHERE [{1}] = @id;", GetAliasName(), GetKeyProperty().Name), new { id = GetKeyProperty().GetValue(entity) });

            }
        }

        public T Update(T entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
UPDATE [{0}]
SET ", GetAliasName());
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("[{0}] = @{0},", prop.Name);
            }

            sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat(" WHERE [{0}] = @{0}", GetKeyProperty().Name);
            using (db = GetConnection())
            {

                db.Execute(sql.ToString(), entity);
                return entity;
            }
        }

        public IEnumerable<E> Query<E>(string query, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Query<E>(query, param, commandTimeout: 5000000);
            }
        }
        public E QueryFirst<E>(string query, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.QueryFirstOrDefault<E>(query, param, commandTimeout: 5000000);
            }
        }
        public SqlMapper.GridReader QueryMultiple(string query, object param = null, string connectionName = null)
        {
            var db = GetConnection(connectionName);
            return db.QueryMultiple(query, param, commandTimeout: 5000000);
        }
        public int Execute(string sql, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Execute(sql, param, commandTimeout: 5000000);
            }
        }

        public E ExecuteScalar<E>(string sql, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.ExecuteScalar<E>(sql, param, commandTimeout: 5000000);
            }
        }

        public void BulkInsert<E>(IEnumerable<E> data, string connectionName = null, List<string> excludedProperties = null)
        {
            using (SqlConnection connection = GetConnection(connectionName))
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );
                // set the destination table name
                bulkCopy.DestinationTableName = GetAliasName();
                connection.Open();
                // write the data in the "dataTable"
                var table = ToDataTable(data, excludedProperties);
                foreach (DataColumn col in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulkCopy.WriteToServer(table);
                connection.Close();
            }
        }
        public DataTable ToDataTable<E>(IEnumerable<E> data, List<string> excludedProperties = null)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(E));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (excludedProperties != null && excludedProperties.Contains(prop.Name))
                    continue;
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (E item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (excludedProperties != null && excludedProperties.Contains(prop.Name))
                        continue;
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public T1 GetById<T1>(int? id, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            if (id == null || id == 0)
            {
                return null;
            }
            using (var db = GetConnection(connectionName))
            {
                return db.Get<T1>(id, commandTimeout: 5000000);
            }
        }

        public T1 GetBy<T1>(IPredicate where, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T1>(where, commandTimeout: 5000000).FirstOrDefault();
            }
        }

        public T GetBy(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null)
        {
            return db.GetList<T>(where, transaction: tran, commandTimeout: 5000000).FirstOrDefault();
        }

        public T1 GetBy<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            return db.GetList<T1>(where, transaction: tran, commandTimeout: 5000000).FirstOrDefault();
        }

        public int Count<T1>(int? id) where T1 : class, IEntityKey, new()
        {
            var predicate = Predicates.Field<T1>(i => i.Id, Operator.Eq, id);
            return Count(predicate);
        }

        public int Count(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Count<T>(where, commandTimeout: 5000000);
            }
        }

        public bool Exists<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            return db.Count<T1>(where, tran, commandTimeout: 5000000) > 0;
        }
        public bool Exists<T1>(IPredicate where, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            return db.Count<T1>(where, commandTimeout: 5000000) > 0;
        }

        public dynamic Insert<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            var result = db.Insert(entity, tran, commandTimeout: 5000000);

            return result;
        }

        public bool InsertWithTran<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            db.Insert<T1>(entities, tran, commandTimeout: 5000000);
            return true;
        }

        public IEnumerable<dynamic> InsertAndGetIds<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            var insertedIds = new List<dynamic>();
            foreach (var entity in entities)
            {
                var result = db.Insert(entity, tran, commandTimeout: 5000000);
                insertedIds.Add(result);
            }
            return insertedIds;
        }

        public T GetDapper(IPredicate pg, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(pg, commandTimeout: 5000000).FirstOrDefault();
            }
        }
        public T1 GetDapper<T1>(IPredicate pg, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T1>(pg, commandTimeout: 5000000).FirstOrDefault();
            }
        }

        public dynamic InsertDapper(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Insert(entity, commandTimeout: 5000000);
            }
        }
        public dynamic InsertDapper<T1>(T1 entity, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Insert(entity, commandTimeout: 5000000);
            }
        }
        public IEnumerable<dynamic> InsertAndGetIds(IEnumerable<T> entities, string connectionName = null)
        {
            var insertedIds = new List<dynamic>();
            using (var db = GetConnection(connectionName))
            {
                foreach (var entity in entities)
                {
                    var result = db.Insert(entity, commandTimeout: 5000000);
                    insertedIds.Add(result);
                }
            }
            return insertedIds;
        }

        public void InsertDapper(IEnumerable<T> entities, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                db.Insert(entities, commandTimeout: 5000000);
            }
        }
        public bool UpdateDapper(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Update(entity, commandTimeout: 5000000);
            }
        }
        public bool UpdateDapper<T1>(T1 entity, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Update(entity, commandTimeout: 5000000);
            }
        }

        public bool DeleteBy(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Delete<T>(where, commandTimeout: 5000000);
            }
        }
        public bool DeleteBy<T1>(IPredicate pg, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Delete<T1>(pg, commandTimeout: 5000000);
            }
        }

        public bool DeleteDapper(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Delete(entity, commandTimeout: 5000000);
            }
        }
        public bool DeleteDapper(int? id, string connectionName = null)
        {
            if (id == null || id == 0)
            {
                return false;
            }
            using (var db = GetConnection(connectionName))
            {
                var item = db.Get<T>(id, commandTimeout: 5000000);
                return db.Delete(item, commandTimeout: 5000000);
            }
        }
        public T GetByIdDapper(int? id, string connectionName = null)
        {
            if (id == null || id == 0)
            {
                return null;
            }
            using (var db = GetConnection(connectionName))
            {
                return db.Get<T>(id, commandTimeout: 5000000);
            }
        }

        public IEnumerable<T> GetAllDapper(string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(null, commandTimeout: 5000000).ToList();
            }
        }
        public async Task<IEnumerable<T>> GetListByAsync(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return (await db.GetListAsync<T>(where, commandTimeout: 5000000)).ToList();
            }
        }
        public IEnumerable<T1> GetAllDapper<T1>(string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T1>(null, commandTimeout: 5000000).ToList();
            }
        }

        public IEnumerable<T> GetListBy(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(where, commandTimeout: 5000000).ToList();
            }
        }

        public T GetBy(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(where, commandTimeout: 5000000).FirstOrDefault();
            }
        }

        public IEnumerable<T1> GetListBy<T1>(IPredicate where, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T1>(where, commandTimeout: 5000000).ToList();
            }
        }
        public bool Exists(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Count<T>(where, commandTimeout: 5000000) > 0;
            }
        }

        public bool Delete<T1>(int id, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            var success = false;
            if (id <= 0)
            {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, id);
            using (var db = GetConnection(connectionName))
            {
                success = db.Delete(predicate, commandTimeout: 5000000);
                return success;
            }
        }

        public bool Delete<T1>(IEnumerable<int> ids, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            var success = true;
            if (ids == null || ids.Any() == false)
            {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, ids);
            using (var db = GetConnection(connectionName))
            {
                success = db.Delete(predicate, commandTimeout: 5000000);
                return success;
            }
        }
        public bool DeleteWithTran<T1>(IEnumerable<int> ids, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            var success = true;
            if (ids == null || ids.Any() == false)
            {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, ids);
            success = db.Delete<T1>(predicate, tran, commandTimeout: 5000000);
            return success;
        }

        #region Async Methods
        public async Task<T> AddAsync(T entity)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat(@"
DECLARE @EntityId BIGINT
INSERT INTO [{0}](", GetAliasName());
            var isKeyAutoNumber = GetKeyAttribute().AutoNumber;
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat("[{0}],", GetKeyProperty().Name);
            }
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("[{0}],", prop.Name);
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(") VALUES(");
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat("@{0},", GetKeyProperty().Name);
            }
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("@{0},", prop.Name);
            }
            sql.Remove(sql.Length - 1, 1);
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat(@"); 
SELECT *
FROM {0}
WHERE {1} = '{2}'
", GetAliasName(), GetKeyProperty().Name, GetKeyProperty().GetValue(entity));
            }
            else
            {
                sql.AppendFormat(@"); 
SET @EntityId = SCOPE_IDENTITY();
SELECT *
FROM {0}
WHERE {1} = @EntityId
", GetAliasName(), GetKeyProperty().Name);
            }

            using (db = GetConnection())
            {
                entity = await db.QueryFirstOrDefaultAsync<T>(sql.ToString(), entity);
                return entity;
            }

        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            using (db = GetConnection())
            {
                return await db.QueryAsync<T>("SELECT * FROM " + GetAliasName());
            }
        }

        public async Task<T> GetAsync(string id)
        {
            var db = GetDbInstance();
            var query = await db.Query(GetAliasName()).Where(GetKeyProperty().Name, id).GetAsync<T>();
            return query.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAsync(int pageIndex, int pageSize)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
WITH CTE
AS
(
SELECT *,ROW_NUMBER() OVER(ORDER BY {1}) RowNum
FROM [{0}]
)
SELECT *
FROM CTE
WHERE RowNum BETWEEN @Start AND @End;", GetAliasName(), GetKeyProperty().Name);
            using (db = GetConnection())
            {
                return await db.QueryAsync<T>(sql.ToString(), new { Start = pageIndex - 1, End = ((pageIndex - 1) * pageSize) + pageSize });
            }
        }

        public async Task RemoveAsync(string id)
        {
            using (db = GetConnection())
            {
                await db.ExecuteAsync(string.Format("DELETE FROM [{0}] WHERE [{1}] = @id;", GetAliasName(), GetKeyProperty().Name), new { id = id });
            }
        }

        public async Task RemoveAsync(T entity)
        {
            using (db = GetConnection())
            {
                await db.ExecuteAsync(string.Format("DELETE FROM [{0}] WHERE [{1}] = @id;", GetAliasName(), GetKeyProperty().Name), new { id = GetKeyProperty().GetValue(entity) });
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
UPDATE [{0}]
SET ", GetAliasName());
            foreach (var prop in GetMappedProperties(true))
            {
                sql.AppendFormat("[{0}] = @{0},", prop.Name);
            }

            sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat(" WHERE [{0}] = @{0}", GetKeyProperty().Name);
            using (db = GetConnection())
            {

                await db.ExecuteAsync(sql.ToString(), entity);
                return entity;
            }
        }

        public async Task<IEnumerable<E>> QueryAsync<E>(string query, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.QueryAsync<E>(query, param, commandTimeout: 5000000);
            }
        }
        public async Task<E> QueryFirstAsync<E>(string query, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.QueryFirstOrDefaultAsync<E>(query, param, commandTimeout: 5000000);
            }
        }
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string query, object param = null, string connectionName = null)
        {
            var db = GetConnection(connectionName);
            return await db.QueryMultipleAsync(query, param, commandTimeout: 5000000);
        }
        public async Task<int> ExecuteAsync(string sql, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.ExecuteAsync(sql, param, commandTimeout: 5000000);
            }
        }

        public async Task<E> ExecuteScalarAsync<E>(string sql, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.ExecuteScalarAsync<E>(sql, param, commandTimeout: 5000000);
            }
        }

        public async Task BulkInsertAsync<E>(IEnumerable<E> data, string tableName, string connectionName = null, List<string> excludedProperties = null)
        {
            using (SqlConnection connection = GetConnection(connectionName))
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );
                // set the destination table name
                bulkCopy.DestinationTableName = tableName;
                connection.Open();
                // write the data in the "dataTable"
                var table = ToDataTable(data, excludedProperties);
                foreach (DataColumn col in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                await bulkCopy.WriteToServerAsync(table);
                connection.Close();
            }
        }
        #endregion


        private string GetAliasName()
        {
            var objType = typeof(T);
            object[] attributes = objType.GetCustomAttributes(true);
            foreach (var attr in attributes)
            {
                AliasAttribute alias = attr as AliasAttribute;
                if (alias != null)
                    return alias.Name;
            }
            return objType.Name;
        }

        private KeyAttribute GetKeyAttribute()
        {
            var objType = typeof(T);
            PropertyInfo[] props = objType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    KeyAttribute key = attr as KeyAttribute;
                    if (key != null)
                    {
                        return key;
                    }
                }
            }
            return null;
        }

        private PropertyInfo GetKeyProperty()
        {
            var objType = typeof(T);
            PropertyInfo[] props = objType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    KeyAttribute key = attr as KeyAttribute;
                    if (key != null)
                    {
                        return prop;
                    }
                }
            }
            return null;
        }

        private IEnumerable<PropertyInfo> GetMappedProperties(bool excludeKey)
        {
            var objType = typeof(T);
            List<PropertyInfo> mappedProperties = new List<PropertyInfo>();
            PropertyInfo[] props = objType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                bool notMapped = false;
                bool isKey = false;
                foreach (object attr in attrs)
                {
                    NotMappedAttribute notMappedAttr = attr as NotMappedAttribute;
                    if (notMappedAttr != null)
                    {
                        notMapped = true;
                        break;
                    }
                    else
                    {
                        KeyAttribute key = attr as KeyAttribute;
                        if (key != null)
                        {
                            isKey = true;
                        }
                    }
                }
                if (!notMapped)
                {
                    if (isKey && excludeKey)
                        continue;
                    mappedProperties.Add(prop);
                }
            }
            return mappedProperties;
        }
    }
}
