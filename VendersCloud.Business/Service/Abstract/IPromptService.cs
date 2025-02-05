using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IPromptService
    {
        Task<JobPostingResponse> GenerateUpdatedContent(PromptRequest request);
    }
}
