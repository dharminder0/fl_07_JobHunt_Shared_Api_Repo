using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Data.Repositories.Concrete
{
    public class PromptRepository : StaticBaseRepository<Prompts>, IPromptRepository
    {
        public PromptRepository(IConfiguration configuration) : base(configuration)
        {
            
                
        }
    }
}
