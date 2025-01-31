using VendersCloud.Business.Entities.RequestModels;

namespace VendersCloud.Business.Service.Abstract
{
    public interface IOrganizationService
    {
        Task<string> RegisterNewOrganizationAsync(RegistrationRequest request);
    }
}
