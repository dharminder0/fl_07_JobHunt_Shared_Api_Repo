﻿using DapperExtensions.Predicate;
using DapperExtensions;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserCompanyMappingRepository : DataRepository<UserCompanyMapping>, IUserCompanyMappingRepository
    {
        public async Task<UserCompanyMapping> GetMappingsByUserIdAsync(string userId)
        {
            try
            {
                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<UserCompanyMapping>(ucm => ucm.UserId, Operator.Eq, userId));

                var userCompanyMapping = await GetListByAsync(pg);
                return userCompanyMapping.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserCompanyMapping>> GetMappingsByCompanyCodeAsync(string companyCode)
        {
            try
            {
                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<UserCompanyMapping>(ucm => ucm.CompanyCode, Operator.Eq, companyCode));

                var userCompanyMapping = await GetListByAsync(pg);
                return userCompanyMapping.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> AddMappingAsync(string userId, string companyCode)
        {
            try
            {
                var sql = @"INSERT INTO UserCompanyMapping (UserId, CompanyCode) VALUES (@userId, @companyCode)";
                await ExecuteAsync(sql, new { userId, companyCode });
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error during mapping insertion", ex);
            }
        }

    }
}
