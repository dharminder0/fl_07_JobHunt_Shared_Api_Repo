using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class BenchService : IBenchService
    {
        private readonly IBenchRepository _benchRepository;
        public BenchService(IBenchRepository benchRepository)
        {
            _benchRepository = benchRepository;
        }

        public async Task<ActionMessageResponse> UpsertBenchAsync(BenchRequest benchRequest)
        {
            try
            {
                if(string.IsNullOrEmpty(benchRequest.OrgCode)|| string.IsNullOrEmpty(benchRequest.FirstName)||string.IsNullOrEmpty(benchRequest.Email))
                {
                    return new ActionMessageResponse()
                    {
                        Success = false,
                        Message = "Enter Valid Inputs",
                        Content = ""
                    };
                }
                var res= await _benchRepository.UpsertBenchMembersAsync(benchRequest);
                if(res)
                    return new ActionMessageResponse()
                    {
                        Success = true,
                        Message = "Bench Member added",
                        Content = ""
                    };
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = "Bench Member not added",
                    Content = ""
                };
            }
            catch (Exception ex) {
                return new ActionMessageResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Content = ""
                };
            }
        }

        public async Task<List<Resources>> GetBenchListAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgCode))
                {
                    throw new Exception("Enter Valid Input");
                }
                return await _benchRepository.GetBenchResponseListAsync(orgCode);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
