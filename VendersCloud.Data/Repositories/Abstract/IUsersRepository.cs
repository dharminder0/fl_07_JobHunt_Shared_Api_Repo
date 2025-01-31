using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface IUsersRepository : IBaseRepository<Users>
    {
        Task<string> InsertUserAsync(RegistrationRequest request, string hashedPassword, byte[] salt, string orgCode);
    }
}
