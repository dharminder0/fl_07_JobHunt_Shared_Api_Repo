using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IRequirementVendorsService
    {
        Task<bool> AddRequirementShareData(RequirementSharedRequest request);
    }
}
