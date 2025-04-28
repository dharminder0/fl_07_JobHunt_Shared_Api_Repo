namespace VendersCloud.Data.Repositories.Concrete
{
    public class PartnerVendorRelRepository : StaticBaseRepository<PartnerVendorRel>, IPartnerVendorRelRepository
    {
        public PartnerVendorRelRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<PartnerVendorRel> GetByIdAsync(int id)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>
                    {
                        Predicates.Field<PartnerVendorRel>(f => f.Id, Operator.Eq, id),
                        Predicates.Field<PartnerVendorRel>(f => f.IsDeleted, Operator.Eq, false),
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PartnerVendorRel>> GetAllAsync()
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM PartnerVendorRel WHERE IsDeleted = 0";
                var result = await dbInstance.SelectAsync<PartnerVendorRel>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdatePartnerVendorRelByIdAsync(int id, PartnerVendorRel updatedEntity)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<PartnerVendorRel>();

                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                   
                    PartnerCode = updatedEntity.PartnerCode,
                    VendorCode = updatedEntity.VendorCode,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedBy = updatedEntity.CreatedBy,
                    UpdatedBy = updatedEntity.UpdatedBy,
                    StatusId = updatedEntity.StatusId  ,                    
                    CreatedOn = updatedEntity.CreatedOn,
                 
                }).Where("Id", id);

                var res = await dbInstance.ExecuteAsync(updateQuery);
                return res != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<int> AddPartnerVendorRelAsync(PartnerVendorRel entity)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<PartnerVendorRel>();

                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    PartnerCode = entity.PartnerCode,
                    VendorCode = entity.VendorCode,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy,
                    StatusId = entity.StatusId,
                    CreatedOn = entity.CreatedOn,
                });

                int insertedId = await dbInstance.ExecuteScalarAsync<int>(insertQuery);
                return insertedId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<PartnerVendorRel>> GetByPartnerIdAsync(string partnerCode, string vendorCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM PartnerVendorRel WHERE PartnerCode = @PartnerCode  and vendorCode = @vendorCode AND IsDeleted = 0";

                var result = await dbInstance.SelectAsync<PartnerVendorRel>(sql, new { partnerCode,vendorCode });
                return result.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

      
    }
}
