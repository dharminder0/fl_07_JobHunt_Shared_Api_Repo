using DapperExtensions;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using VendersCloud.Business.Entities.Abstract;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Caching;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class BaseRepository<T> : DataRepository<T>, IBaseRepository<T> where T : class, IEntityKey, new() {
        public BaseRepository(IConfiguration configuration) : base(configuration) { }

        public bool DeleteWithTran<T1>(IPredicate predicate, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
         
           var success = db.Delete<T1>(predicate, tran, commandTimeout: 5000000);
            return success;
        }

        public bool ClearCacheKeysByText(string text) {
            var result = false;
            try {
                var allKeys = AppLocalCache.GetAllKeys();
                var entityFieldsKeys = allKeys.Where(x => x.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1).ToList();
                foreach (var entityFieldsKey in entityFieldsKeys) {
                    AppLocalCache.Remove(entityFieldsKey);
                }

                result = true;
            }
            catch (Exception e) {

            }

            return result;
        }

        public bool ClearAllClientCache(string clientCode) {
            return ClearCacheKeysByText(clientCode);
        }

        public T Upsert(T entity) {
            var predicate = Predicates.Field<T>(i => i.Id, Operator.Eq, entity.Id);
            var isExist = Exists(predicate);
            if (entity as IAudit != null) {
                if (isExist) {
                    var existingEntity = GetById<T>(entity.Id);
                    ((IAudit)entity).UpdatedAt = DateTime.UtcNow;
                    var entityAudit = ((IAudit)existingEntity);
                    ((IAudit)entity).CreatedAt = entityAudit.CreatedAt ?? entityAudit.UpdatedAt ?? DateTime.UtcNow;
                }
                else {
                    ((IAudit)entity).CreatedAt = DateTime.UtcNow;
                }
            }

            if (isExist) {
                var isSuccess = UpdateDapper(entity);
            }
            else {
                var entityId = InsertDapper(entity);
                entity.Id = int.Parse(entityId.ToString());
            }

            return entity;
        }

        public async Task<T> UpsertAsync(T entity) {
            var predicate = Predicates.Field<T>(i => i.Id, Operator.Eq, entity.Id);
            var isExist = Exists(predicate);
            if (entity as IAudit != null) {
                if (isExist) {
                    var existingEntity = GetById<T>(entity.Id);
                    ((IAudit)entity).UpdatedAt = DateTime.UtcNow;
                    var entityAudit = ((IAudit)existingEntity);
                    ((IAudit)entity).CreatedAt = entityAudit.CreatedAt ?? entityAudit.UpdatedAt ?? DateTime.UtcNow;
                }
                else {
                    ((IAudit)entity).CreatedAt = DateTime.UtcNow;
                }
            }

            if (isExist) {
                var isSuccess = await UpdateDapperAsync(entity);
            }
            else {
                var entityId = await InsertDapperAsync(entity);
                entity.Id = int.Parse(entityId.ToString());
            }

            return entity;
        }

        public T1 GetById<T1>(int? id, string connectionName = null) where T1 : class, IEntityKey, new() {
            if (id == null || id == 0) {
                return null;
            }
            using (var db = GetConnection(connectionName)) {
                return db.Get<T1>(id, commandTimeout: 5000000);
            }
        }

        public T1 GetBy<T1>(IPredicate where, string connectionName = null) where T1 : class, IEntityKey, new() {
            using (var db = GetConnection(connectionName)) {
                return db.GetList<T1>(where, commandTimeout: 5000000).FirstOrDefault();
            }
        }

        public T GetBy(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) {
            return db.GetList<T>(where, transaction: tran, commandTimeout: 5000000).FirstOrDefault();
        }

        public T1 GetBy<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            return db.GetList<T1>(where, transaction: tran, commandTimeout: 5000000).FirstOrDefault();
        }

        public IEnumerable<T1> GetListBy<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new()
        {
            return db.GetList<T1>(where, transaction: tran, commandTimeout: 5000000);
        }

        public int Count<T1>(int? id) where T1 : class, IEntityKey, new() {
            var predicate = Predicates.Field<T1>(i => i.Id, Operator.Eq, id);
            return Count(predicate);
        }


        public T1 UpsertWithTran<T1>(T1 entity, SqlConnection db, SqlTransaction tran, int? userId = null) where T1 : class, IEntityKey, new() {
            var predicate = Predicates.Field<T1>(i => i.Id, Operator.Eq, entity.Id);
            var isExist = Exists<T1>(predicate, db, tran);
            if (entity as IEntityAudit != null) {
                if (isExist) {
                    var existingEntity = GetById<T1>(entity.Id);
                    ((IEntityAudit)entity).UpdatedAt = DateTime.UtcNow;
                    var entityAudit = ((IEntityAudit)existingEntity);
                    ((IEntityAudit)entity).UpdatedBy = userId ?? entityAudit.UpdatedBy;
                    ((IEntityAudit)entity).CreatedAt = entityAudit.CreatedAt?? entityAudit.UpdatedAt ?? DateTime.UtcNow;
                    ((IEntityAudit)entity).CreatedBy = entityAudit.CreatedBy?? userId ?? entityAudit.UpdatedBy;
                }
                else {
                    ((IEntityAudit)entity).CreatedAt = DateTime.UtcNow;
                    ((IEntityAudit)entity).CreatedBy = userId ?? ((IEntityAudit)entity).CreatedBy;
                }
            }
            else if (entity as IAudit != null) {
                if (isExist) {
                    var existingEntity = GetById<T1>(entity.Id);
                    ((IAudit)entity).UpdatedAt = DateTime.UtcNow;
                    var entityAudit = ((IAudit)existingEntity);
                    ((IAudit)entity).CreatedAt = entityAudit.CreatedAt ?? entityAudit.UpdatedAt ?? DateTime.UtcNow;
                }
                else {
                    ((IAudit)entity).CreatedAt = DateTime.UtcNow;
                }
            }
            if (isExist) {
                var isSuccess = Update(entity, db, tran);
            }
            else {
                var entityId = Insert(entity, db, tran);
                entity.Id = int.Parse(entityId.ToString());
            }

            return entity;
        }

        public T1 DeleteWithTran<T1>(T1 entity, SqlConnection db, SqlTransaction tran, int? userId = null) where T1 : class, IEntityKey, new() {
            var predicate = Predicates.Field<T1>(i => i.Id, Operator.Eq, entity.Id);
            var isExist = Exists<T1>(predicate, db, tran);
            if (entity as IEntityAudit != null) {
                if (isExist) {
                    var existingEntity = GetById<T1>(entity.Id);
                    ((IEntityAudit)entity).DeletedAt = DateTime.UtcNow;
                    ((IEntityAudit)entity).DeletedBy = userId ?? ((IEntityAudit)existingEntity).DeletedBy;
                    var isSuccess = Update(entity, db, tran);
                }
            }
            return entity;
        }

        public dynamic Insert<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var result = db.Insert(entity, tran, commandTimeout: 5000000);

            return result;
        }
        public async Task<dynamic> InsertAsync<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var result = await db.InsertAsync(entity, tran, commandTimeout: 5000000);

            return result;
        }

        public bool Exists<T1>(IPredicate where, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            return db.Count<T1>(where, tran, commandTimeout: 5000000) > 0;
        }

        public bool InsertWithTran<T1>(T1 entities, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            db.Insert<T1>(entities, tran, commandTimeout: 5000000);
            return true;
        }

        public IEnumerable<dynamic> InsertAndGetIds<T1>(IEnumerable<T1> entities, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var insertedIds = new List<dynamic>();
            foreach (var entity in entities) {
                var result = db.Insert(entity, tran, commandTimeout: 5000000);
                insertedIds.Add(result);
            }
            return insertedIds;
        }

        public bool Update<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var result = db.Update(entity, tran, commandTimeout: 5000000);

            return result;
        }
        public async Task<bool> UpdateAsync<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var result = await db.UpdateAsync(entity, tran, commandTimeout: 5000000);

            return result;
        }

        public async Task<bool> DeleteAsync<T1>(T1 entity, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var result = await db.DeleteAsync(entity, tran, commandTimeout: 5000000);
            return result;
        }

        public bool Delete<T1>(int id, string connectionName = null) where T1 : class, IEntityKey, new() {
            var success = false;
            if (id <= 0) {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, id);
            using (var db = GetConnection(connectionName)) {
                success = db.Delete(predicate, commandTimeout: 5000000);
                return success;
            }
        }

        public bool Delete<T1>(IEnumerable<int> ids, string connectionName = null) where T1 : class, IEntityKey, new() {
            var success = true;
            if (ids == null || ids.Any() == false) {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, ids);
            using (var db = GetConnection(connectionName)) {
                success = db.Delete(predicate, commandTimeout: 5000000);
                return success;
            }
        }
        public bool DeleteWithTran<T1>(IEnumerable<int> ids, SqlConnection db, SqlTransaction tran, string connectionName = null) where T1 : class, IEntityKey, new() {
            var success = true;
            if (ids == null || ids.Any() == false) {
                return success;
            }
            var predicate = Predicates.Field<T1>(f => f.Id, Operator.Eq, ids);
            success = db.Delete<T1>(predicate, tran, commandTimeout: 5000000);
            return success;
        }

        private bool TrySetProperty(object obj, string property, object value) {
            var prop = obj.GetType().GetProperty(property, BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite) {
                prop.SetValue(obj, value, null);
                return true;
            }

            return false;
        }
    }
}
