using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class BenchMatchRecord
    {
        public int ResourcesId { get; set; }
        public string OrgCode { get; set; }
        public string SearchText { get; set; }
        public  List<int> Status { get; set; }
        public List<int> LocationType { get; set; }
    }
}
