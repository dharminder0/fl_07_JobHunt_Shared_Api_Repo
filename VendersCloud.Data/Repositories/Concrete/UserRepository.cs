using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class UserRepository : DataRepository<User>,IUserRepository
    {

        public async Task<IEnumerable<User>> GetAllUsersInfo()
        {
            try
            {
                var sql = @"SELECT * FROM [USER]";
                return await QueryAsync<User>(sql);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging library)
                Console.WriteLine(ex.Message);
                throw; // Rethrow the original exception
            }
        }


    }
}
