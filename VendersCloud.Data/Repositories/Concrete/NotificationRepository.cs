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
        public async Task< List<Notifications>> GetNotificationsAsync(string orgCode)
        {

            var dbInstance = GetDbInstance();
            var sql = @"SELECT * FROM Notifications 
                    WHERE orgCode = @orgCode  and isread=0
                    ORDER BY CreatedOn ASC";
            var history = dbInstance.Select<Notifications>(sql, new { orgCode }).ToList();
            return history;
        }
        public async Task<bool> UpsertNotificationAsync(int notificationId, bool isRead)
        {
            try
            {
                var dbInstance = GetDbInstance();

                var updateQuery = new Query("Notifications")
                    .Where("Id", notificationId)
                    .AsUpdate(new
                    {
                        IsRead = isRead,
                     
                    });

                var rowsAffected = await dbInstance.ExecuteAsync(updateQuery);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
              
                return false;
            }
        }

    }
}
