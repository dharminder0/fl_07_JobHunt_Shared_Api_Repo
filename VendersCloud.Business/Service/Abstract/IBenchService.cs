using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IBenchService
    {
        Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest);
        Task<List<Resources>> GetBenchListAsync(string orgCode);
    }
}
