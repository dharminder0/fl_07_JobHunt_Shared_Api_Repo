using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersInfo();
    }
}
