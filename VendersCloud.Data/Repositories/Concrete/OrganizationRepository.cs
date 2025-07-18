﻿namespace VendersCloud.Data.Repositories.Concrete
{
    public class OrganizationRepository : StaticBaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<string> RegisterNewOrganizationAsync(RegistrationRequest request, string orgCode)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Organization>();

                var query = new Query(tableName.TableName)
                    .Where("IsDeleted", false)
                    .Where("OrgName", request.CompanyName)
                    .Select("OrgCode");

                var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);
                if (!string.IsNullOrEmpty(existingOrgCode))
                {
                    return existingOrgCode;
                }
                var insertQuery = new Query(tableName.TableName).AsInsert(new
                {
                    OrgCode = orgCode,
                    OrgName = request.CompanyName,
                    Email = request.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    IsDeleted = false
                });

                var insertedOrgCode = await dbInstance.ExecuteScalarAsync<string>(insertQuery);
                var query2 = new Query(tableName.TableName)
                    .Where("Email", request.Email)
                    .Where("IsDeleted", false)
                    .Select("OrgCode");

                var existingOrgCode2 = await dbInstance.ExecuteScalarAsync<string>(query);

                if (!string.IsNullOrEmpty(existingOrgCode2))
                {
                    return existingOrgCode2;
                }
                return insertedOrgCode; // Return the newly inserted OrgCode
            }
            catch (Exception ex)
            {
                // Log the exception properly
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<Organization> GetOrganizationData(string orgCode)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Organization>(f=>f.OrgCode,Operator.Eq,orgCode),
                        Predicates.Field<Organization>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Organization> GetOrganizationDataByIdAsync(int Id)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<Organization>(f=>f.Id,Operator.Eq,Id),
                        Predicates.Field<Organization>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Organization>> GetOrganizationListAsync()
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Organization";

                var orgdata = dbInstance.Select<Organization>(sql).ToList();
                return orgdata;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                // Log the exception (optional)
                return null;
            }
        }

        public async Task<bool> UpdateOrganizationByOrgCodeAsync(CompanyInfoRequest infoRequest, string orgCode,string uploadedimageUrl)
        {
            try
            {
                var dbInstance = GetDbInstance();
                var tableName = new Table<Organization>();
                var updateQuery = new Query(tableName.TableName).AsUpdate(new
                {
                    OrgName = infoRequest.OrgName,
                    Email = infoRequest.ContactMail,
                    Description = infoRequest.Portfolio,
                    Website = infoRequest.Website,
                    Phone = infoRequest.Phone,
                    EmpCount = infoRequest.Strength,
                    UpdatedOn = DateTime.UtcNow,
                    Logo= uploadedimageUrl,
                    IsDeleted = false
                }).Where("OrgCode", orgCode);
                var res = await dbInstance.ExecuteAsync(updateQuery);
                if (res != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrganizationAddressByOrgCodeAsync(string regAddress, string orgCode)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Organization>();
            var updateQuery = new Query(tableName.TableName).AsUpdate(new
            {
                RegAddress=regAddress,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false
            }).Where("OrgCode", orgCode);
            var res = await dbInstance.ExecuteAsync(updateQuery);
            if (res != null)
            {
                return true;
            }
            return false;
        }
        public async Task<Users> GetUserByIdAsync(int Id)
        {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM Users Where Id=@Id";

                var users = await dbInstance.SelectAsync<Users>(sql, new { Id });
                return users?.FirstOrDefault();
            
        }
        public async Task<List<Organization>> GetOrgByListAsync(List<string>? orgcode)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Organization Where OrgCode In @orgcode";

            var users = await dbInstance.SelectAsync<Organization>(sql, new { orgcode });
            return users.ToList();

        }
        public async Task<Organization> GetOrganizationByEmailAndOrgCodeAsync(string email, string orgCode)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Organization>(f=>f.Email,Operator.Eq,email),
                        Predicates.Field<Organization>(f=>f.OrgCode,Operator.Eq,orgCode),
                        Predicates.Field<Organization>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });
            
        }

    }

}
