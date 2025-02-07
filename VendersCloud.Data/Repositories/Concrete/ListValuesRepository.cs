using DapperExtensions;
using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class ListValuesRepository : StaticBaseRepository<ListValues>, IListValuesRepository
    {
        public ListValuesRepository(IConfiguration configuration) :base(configuration)
        {

        }

        public async Task<ListValues> GetListValuesByNameAsync(string name)
        {
                return await GetByAsync(new PredicateGroup
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate> {
                        Predicates.Field<ListValues>(f=>f.Value,Operator.Eq,name),
                        Predicates.Field<ListValues>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                });

        }

        public async Task<IList<ListValues>> GetListValuesAsync()
        {

                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM ListValues";

                var ListValues = dbInstance.Select<ListValues>(sql).ToList();
                return ListValues;
      
            
        }

        public async Task<List<ListValues>> GetListValuesByMasterListIdAsync(int mastervalue)
        {
            var dbInstance = GetDbInstance();
            var sql = "SELECT * FROM ListValues Where MasterListId=@mastervalue";

            var ListValues = dbInstance.Select<ListValues>(sql, new { mastervalue }).ToList();
            return ListValues;

        }
    }
}
