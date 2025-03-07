namespace VendersCloud.Data.Repositories.Concrete
{
    public class MasterListRepository : StaticBaseRepository<MasterList>, IMasterListRepository
    {
        public MasterListRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<List<MasterList>>GetMasterListAsync()
        {
            try
            {
                var dbInstance = GetDbInstance();
                var sql = "SELECT * FROM MasterList";

                var masterList = dbInstance.Select<MasterList>(sql).ToList();
                return masterList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddBulkMasterListAsync(List<string> names)
        {
            try
            {
                var dbInstance = GetDbInstance();

                // Create a list of MasterList objects from the list of names (strings)
                var masterLists = names.Select(name => new MasterList { Name = name, IsDeleted = false }).ToList();

                // Insert query using SqlKata's AsInsert method
                foreach (var masterList in masterLists)
                {
                    // Remove the identity column value before inserting
                    var insertQuery = new Query("MasterList").AsInsert(new
                    {
                        masterList.Name,
                        masterList.IsDeleted
                    });

                    // Execute the query asynchronously
                    await dbInstance.ExecuteAsync(insertQuery);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<MasterList> GetMasterListByIdAndNameAsync(string name)
        {
            try
            {
                return await GetByAsync(new PredicateGroup
                    {
                        Operator = GroupOperator.Or,
                        Predicates = new List<IPredicate> {
                        Predicates.Field<MasterList>(f=>f.Name,Operator.Eq,name),
                        Predicates.Field<MasterList>(f=>f.IsDeleted,Operator.Eq,false),
                    }
                  });
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
