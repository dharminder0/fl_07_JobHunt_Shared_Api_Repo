using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class NotificationRepository : StaticBaseRepository<Notifications>, INotificationRepository  
    {
        public NotificationRepository(IConfiguration configuration) : base(configuration)
        {
                
        }
        public async Task<bool> InsertNotificationAsync(string orgCode, string message,int type)
        {
            var dbInstance = GetDbInstance();
            var tableName = new Table<Notifications>(); 

            var insertQuery = new Query(tableName.TableName).AsInsert(
                new
                {
                    OrgCode = orgCode,         
                    Message = message,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false,
                    NotificationType = type
                });

            await dbInstance.ExecuteAsync(insertQuery);
            return true;
        }

    }
}
