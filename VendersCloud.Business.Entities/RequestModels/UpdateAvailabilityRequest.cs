using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class UpdateAvailabilityRequest
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public int Availability { get; set; }
    }

}
