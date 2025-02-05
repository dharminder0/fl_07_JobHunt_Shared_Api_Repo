﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Entities.ResponseModels
{
    public class JobPostingResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Budget { get; set; }
        public string Positions { get; set; }
        public string ContractPeriod { get; set; }
        public string Location_Type { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
    }
}
