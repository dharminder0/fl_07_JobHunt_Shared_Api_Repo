using DapperExtensions;
using Microsoft.Extensions.Configuration;
using SqlKata;
using System.Xml.Linq;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class ClientsRepository : StaticBaseRepository<Clients>, IClientsRepository
    {
        public ClientsRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<bool> UpsertClientAsync(ClientsRequest request, string clientCode)
        {
            var dbInstance = GetDbInstance();
            var table = new Table<Clients>();
            var query = new Query(table.TableName)
                        .Where("ClientName", request.ClientName)
                        .Where("OrgCode", request.OrgCode)
                        .Select("Id");
            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                var updateQuery = new Query(table.TableName).AsUpdate(new
                {
                    Description = request.Description,
                    ContactPhone = request.ContactPhone,
                    ContactEmail = request.ContactEmail,
                    Address = request.Address,
                    Website = request.Website,
                    LogoURL = request.LogoURL,
                    FaviconURL = request.FaviconURL,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = "",
                    Status = request.Status,
                    isDeleted = false
                }).Where("ClientName", request.ClientName);

                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                return true;
            }

            var insertQuery = new Query(table.TableName).AsInsert(new
            {
                ClientCode = clientCode,
                OrgCode = request.OrgCode,
                ClientName = request.ClientName,
                Description = request.Description,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.ContactEmail,
                Address = request.Address,
                Website = request.Website,
                LogoURL = request.LogoURL,
                FaviconURL = request.FaviconURL,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "",
                Status = request.Status,
                isDeleted = false
            });
            await dbInstance.ExecuteScalarAsync<string>(insertQuery);
            return true;
        }

        public async Task<Clients> GetClientsByIdAsync(int id)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.Id,Operator.Eq,id),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }

        public async Task<Clients> GetClientsByNameAsync(string name)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.ClientName,Operator.Eq,name),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }
        public async Task<List<Clients>> GetClientsByOrgCodeAsync(string orgCode)
        {

            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM Clients Where OrgCode=@orgCode And isDeleted=0";

            var clients = dbInstance.Select<Clients>(sql, new { orgCode }).ToList();
            return clients;

        }
        public async Task<Clients> GetClientsByClientCodeAsync(string clientCode)
        {

            return await GetByAsync(new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate> {
                        Predicates.Field<Clients>(f=>f.ClientCode,Operator.Eq,clientCode),
                        Predicates.Field<Clients>(f=>f.IsDeleted,Operator.Eq,false),
                    }
            });

        }

        public async Task<bool> DeleteClientsByIdAsync(string orgCode, int id, string clientName)
        {
            var dbInstance = GetDbInstance();
            var table = new Table<Clients>();
            var query = new Query(table.TableName)
                        .Where("ClientName", clientName)
                        .Where("OrgCode", orgCode)
                        .Select("Id");
            var existingOrgCode = await dbInstance.ExecuteScalarAsync<string>(query);

            if (!string.IsNullOrEmpty(existingOrgCode))
            {
                var updateQuery = new Query(table.TableName).AsUpdate(new
                {
                    UpdatedOn = DateTime.UtcNow,
                    isDeleted = true
                }).Where("ClientName", clientName);

                await dbInstance.ExecuteScalarAsync<string>(updateQuery);
                return true;
            }
            return false;
        }
    }
}

