﻿using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
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
                    .Where("Email", request.Email)
                    .Where("IsDeleted",false)
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
            catch(Exception ex)
            {
                return null;
            }
        }

    }

}
