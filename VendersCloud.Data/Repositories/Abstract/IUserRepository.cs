using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Common.Data;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUserRepository :IDataRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersInfo();
    }
}
