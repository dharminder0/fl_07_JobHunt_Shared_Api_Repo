using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendersCloud.Business.Entities.DataModels;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class NotificationRequest
    {
        public string OrgCode { get; set; }
        public string Message { get; set; }
        public int  Type { get; set; }
    }
}
