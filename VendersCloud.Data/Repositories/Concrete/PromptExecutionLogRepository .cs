using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SqlKata;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Data;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class PromptExecutionLogRepository : StaticBaseRepository<PromptExecutionLog>, IPromptExecutionLogRepository
    {
        public PromptExecutionLogRepository(IConfiguration configuration) : base(configuration)
        {

        }
    
    }
}
