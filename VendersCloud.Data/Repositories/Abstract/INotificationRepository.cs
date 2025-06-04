using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Data.Repositories.Abstract
{
    public interface INotificationRepository : IBaseRepository<Notifications>
    {
        Task<bool> InsertNotificationAsync(string orgCode, string message,int type);
    }
}
