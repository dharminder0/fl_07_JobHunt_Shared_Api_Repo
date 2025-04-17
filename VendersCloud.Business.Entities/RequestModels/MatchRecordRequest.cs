using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class MatchRecordRequest
    {
        public List<int> ResourceId { get; set; } 
        public List<int> RequirementId { get; set; }
        public int MatchScore { get; set; }
    }
}
