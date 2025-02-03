using Dapper;
using DapperExtensions;
using IgniteSecurityLib;
using Microsoft.Extensions.Configuration;
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
using VendersCloud.Business.Entities.Abstract;
using VendersCloud.Common.Data;
using VendersCloud.Common.Settings;
using KeyAttribute = VendersCloud.Business.Entities.Abstract.KeyAttribute;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class DataRepository<T> where T : class, new()
    {
        protected SqlConnection db;
        protected string _connectionName;
        public IConfiguration _configuration { get; }
        private SqlServerCompiler _sqlServerCompiler = new SqlServerCompiler();

        public DataRepository(string connectionName, IConfiguration configuration)
        {
            _connectionName = connectionName;
            _configuration = configuration;
        }

        public DataRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionName = _configuration["ConnectionName"];
        }
        public DataRepository(string connectionName)
        {
            _connectionName = connectionName;
        }

        protected SqlConnection GetConnection(string connectionName = null)
        {
            try
            {
                var res= new SqlConnection(_configuration.GetConnectionString(connectionName ?? _connectionName));
                if(res.ConnectionString== "")
                {
                    return GetConnectionv2(connectionName);
                }
                return res;
            }
            catch(Exception ex)
            {
                return GetConnectionv2(connectionName);
            }
        }
        protected SqlConnection GetConnectionv2(string connectionName = null)
        {
            connectionName = connectionName ?? _connectionName;
            var filePath = _configuration["FilePath"];
            var connectionString = string.Empty;

            Console.WriteLine($"ConnectionName: {connectionName}");
            Console.WriteLine($"FilePath: {filePath}");

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("FilePath cannot be null or empty.");
            }

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var jsonObject = JObject.Parse(json);
                Console.WriteLine($"JSON Object: {jsonObject}");

                var connectionStrings = jsonObject["ConnectionStrings"];
                if (connectionStrings != null)
                {
                    Console.WriteLine($"ConnectionStrings: {connectionStrings}");

                    var token = connectionStrings[connectionName];
                    if (token != null)
                    {
                        connectionString = token.ToString();
                        Console.WriteLine($"ConnectionString: {connectionString}");
                    }
                    else
                    {
                        Console.WriteLine($"The given key '{connectionName}' was not present in the ConnectionStrings section of the JSON file.");
                        throw new KeyNotFoundException($"The given key '{connectionName}' was not present in the ConnectionStrings section of the JSON file.");
                    }
                }
                else
                {
                    Console.WriteLine("The ConnectionStrings section is missing in the JSON file.");
                    throw new KeyNotFoundException("The ConnectionStrings section is missing in the JSON file.");
                }
            }
            else
            {
                Console.WriteLine($"The JSON configuration file '{filePath}' does not exist.");
                throw new FileNotFoundException($"The JSON configuration file '{filePath}' does not exist.");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("ConnectionString cannot be null or empty.");
            }

            return new SqlConnection(connectionString);
        }

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

        public string CompileQuery(Query query, bool fillBindings = false)
        {
            var compiledQ = _sqlServerCompiler.Compile(query);
            var result = compiledQ.Sql;
            if (fillBindings)
            {
                //foreach (var binding in compiledQ.NamedBindings) {
                //    result = Regex.Replace(result, $@"(?<![\w\\]){binding.Key}(?!\w)", binding.Value.ToString());
                //}
                result = compiledQ.ToString();
            }
            return result;
        }

        public T Add(T entity, string[] excludedProperties)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat(@"
DECLARE @EId BIGINT
INSERT INTO [{0}](", GetAliasName());
            var isKeyAutoNumber = GetKeyAttribute().AutoNumber;
            if (!isKeyAutoNumber)
            {
                sql.AppendFormat("[{0}],", GetKeyProperty().Name);
            }
            foreach (var prop in GetMappedProperties(true))
            {
                if (excludedProperties != null && excludedProperties.Contains(prop.Name))
                    continue;
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
                if (excludedProperties != null && excludedProperties.Contains(prop.Name))
                    continue;
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
SET @EId = SCOPE_IDENTITY();
SELECT *
FROM {0}
WHERE {1} = @EId
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

        public T Update(T entity, string[] excludedProperties = null)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
UPDATE [{0}]
SET ", GetAliasName());
            foreach (var prop in GetMappedProperties(true))
            {
                if (excludedProperties != null && excludedProperties.Contains(prop.Name))
                    continue;
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

        public IEnumerable<E> ExecuteStoredProcedure<E>(string spName, object param = null, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Query<E>(spName, param, commandType: CommandType.StoredProcedure);
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

        public void BulkInsert<E>(IEnumerable<E> data, string tableName, string connectionName = null, List<string> excludedProperties = null)
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
                var destinationTable = ToDataTable(data, excludedProperties);
                #region compare source & distination
                //Get Column from Source table
                string sourceTableQuery = $"Select top 1 * from {tableName}";

                //i use sql helper for executing query you can use corde sw

                DataTable sourceTable = new DataTable();
                sourceTable.Load(connection.ExecuteReader(sourceTableQuery));
                //DataTable dtSource = connection.ExecuteReader(sourceTableQuery).;
                List<string> diff = new List<string>();
                for (int i = 0; i < destinationTable.Columns.Count; i++)
                {
                    string destinationColumnName = destinationTable.Columns[i].ToString();

                    // check if destination column exists in source table 
                    // Contains method is not case sensitive    
                    if (sourceTable.Columns.Contains(destinationColumnName))
                    {
                        //Once column matched get its index
                        int sourceColumnIndex = sourceTable.Columns.IndexOf(destinationColumnName);

                        string sourceColumnName = sourceTable.Columns[sourceColumnIndex].ToString();
                        diff.Add(sourceColumnName);

                        // give column name of source table rather then destination table 
                        // so that it would avoid case sensitivity
                        bulkCopy.ColumnMappings.Add(sourceColumnName, sourceColumnName);
                    }
                }
                #endregion
                //foreach (DataColumn col in destinationTable.Columns) {
                //    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                //}
                bulkCopy.WriteToServer(destinationTable);
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

        public T GetDapper(IPredicate pg, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.FirstOrDefault<T>(pg, commandTimeout: 5000000);
            }
        }
        public async Task<T> GetDapperAsync(IPredicate pg, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.FirstOrDefaultAsync<T>(pg, commandTimeout: 5000000);
            }
        }
        public T1 GetDapper<T1>(IPredicate pg, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.FirstOrDefault<T1>(pg, commandTimeout: 5000000);
            }
        }
        public async Task<T1> GetDapperAsync<T1>(IPredicate pg, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.FirstOrDefaultAsync<T1>(pg, commandTimeout: 5000000);
            }
        }

        public dynamic InsertDapper(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Insert(entity, commandTimeout: 5000000);
            }
        }
        public async Task<dynamic> InsertDapperAsync(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.InsertAsync(entity, commandTimeout: 5000000);
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
        public void InsertDapper<T1>(IEnumerable<T1> entities, string connectionName = null) where T1 : class, new()
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
        public async Task<bool> UpdateDapperAsync(T entity, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return await db.UpdateAsync(entity, commandTimeout: 5000000);
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
        public async Task<T> GetByIdDapperAsync(int? id, string connectionName = null)
        {
            if (id == null || id == 0)
            {
                return null;
            }
            using (var db = GetConnection(connectionName))
            {
                return await db.GetAsync<T>(id, commandTimeout: 5000000);
            }
        }
        public async Task<T> GetByIdDapperAsync<T>(int? id, string connectionName = null) where T : class, new()
        {
            if (id == null || id == 0)
            {
                return null;
            }
            using (var db = GetConnection(connectionName))
            {
                return await db.GetAsync<T>(id, commandTimeout: 5000000);
            }
        }

        public IEnumerable<T> GetAllDapper(string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(null, commandTimeout: 5000000).ToList();
            }
        }
        public async Task<IEnumerable<T>> GetAllDapperAsync(string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return (await db.GetListAsync<T>(null, commandTimeout: 5000000)).ToList();
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
        public async Task<IEnumerable<T>> GetListByAsync(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return (await db.GetListAsync<T>(where, commandTimeout: 5000000)).ToList();
            }
        }

        public T GetBy(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T>(where, commandTimeout: 5000000).FirstOrDefault();
            }
        }
        public async Task<T> GetByAsync(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return (await db.GetListAsync<T>(where, commandTimeout: 5000000)).FirstOrDefault();
            }
        }

        public IEnumerable<T1> GetListBy<T1>(IPredicate where, string connectionName = null) where T1 : class, new()
        {
            using (var db = GetConnection(connectionName))
            {
                return db.GetList<T1>(where, commandTimeout: 5000000).ToList();
            }
        }

        public int Count(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Count<T>(where, commandTimeout: 5000000);
            }
        }

        //public int Count(IFieldPredicate pred, string connectionName = null) {
        //    using (var db = GetConnection(connectionName)) {
        //        return db.Count<T>(pred, commandTimeout: 5000000);
        //    }
        //}

        public bool Exists(IPredicate where, string connectionName = null)
        {
            using (var db = GetConnection(connectionName))
            {
                return db.Count<T>(where, commandTimeout: 5000000) > 0;
            }
        }

        public string DecryptToken(string token)
        {
            var result = string.Empty;
            var encryptionEnabled = Convert.ToBoolean(_configuration["UserTokenEncryptionEnabled"]);
            if (encryptionEnabled)
            {
                var issuer = _configuration["JwtIssuer"];
                var audience = _configuration["JwtAudience"];
                var symmetricSecretKey = _configuration["SymmetricSecretKey"];
                var decryptedToken = JwtSecurityService.Decrypt(symmetricSecretKey, token);
                if (!string.IsNullOrWhiteSpace(decryptedToken))
                {
                    var userToken = JwtSecurityService.Decode(decryptedToken);
                    return userToken;
                }
            }
            return result;
        }
        public bool InsertWithTran<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, new()
        {
            db.Insert<T1>(entities, tran, commandTimeout: 5000000);
            return true;
        }
        //public bool Exists(IFieldPredicate p, string connectionName = null) {
        //    using (var db = GetConnection(connectionName)) {
        //        return db.Count<T>(p, commandTimeout: 5000000) > 0;
        //    }
        //}

    }
}
