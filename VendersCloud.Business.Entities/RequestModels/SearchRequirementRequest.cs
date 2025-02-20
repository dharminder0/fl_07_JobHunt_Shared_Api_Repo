using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class SearchRequirementRequest
    {
        public string OrgCode { get; set; }
        public string SearchText {  get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public List<int> LocationType {  get; set; }
        public List<int> Status {  get; set; }
        public List<string> ClientCode { get; set; }

    }
}
