using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.RequestModels
{
    public class PromptRequest
    {
        public string  PromptCode { get; set; }
        public int LoginUserId { get; set; }
        public string PromptJson { get; set; }
    }
}
