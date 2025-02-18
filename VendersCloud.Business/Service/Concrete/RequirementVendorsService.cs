using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.Dtos;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class RequirementVendorsService :IRequirementVendorsService
    {
        private readonly IRequirementVendorsRepository _requirementVendorsRepository;
        private readonly IRequirementRepository _requirementRepository;
        public RequirementVendorsService(IRequirementVendorsRepository requirementVendorsRepository,IRequirementRepository requirementRepository)
        {
            _requirementVendorsRepository = requirementVendorsRepository;
            _requirementRepository = requirementRepository;
        }


        public async Task<bool> AddRequirementShareData(RequirementSharedRequest request)
        {
            try
            {
                int id = 0;
                if (string.IsNullOrEmpty(request.RequirementId )) throw new ArgumentNullException("RequirementId can't be null!!");
                var data = await _requirementRepository.GetRequirementListByIdAsync(request.RequirementId);
                if (data == null)throw new ArgumentException("Data Related this Id Is Not Found!!");
                RequirementDto dto= new RequirementDto();
                foreach (var item in data)
                {
                    dto.Title = item.Title;
                    dto.Description = item.Description;
                    dto.OrgCode= item.OrgCode;
                    dto.Experience= item.Experience;
                    dto.Budget= item.Budget;
                    dto.Positions= item.Positions;
                    dto.LocationType= item.LocationType;
                    dto.Location= item.Location;
                    dto.ClientId= item.ClientId;
                    dto.Remarks= item.Remarks;
                    dto.Visibility = request.Visibility;
                    dto.Hot = item.Hot;
                    dto.Status = item.Status;
                    id = item.Id;
                }
                var uniqueId = Guid.NewGuid().ToString().Substring(0, 12);
                var res= await _requirementRepository.RequirementUpsertV2Async(dto, uniqueId);
                if (res)
                {
                    foreach (var orgcode in request.OrgCode)
                    {
                        var response = await _requirementVendorsRepository.AddRequirementVendorsDataAsync(id, orgcode);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex) {
                throw ex;
            }
        
        }
    }
}
