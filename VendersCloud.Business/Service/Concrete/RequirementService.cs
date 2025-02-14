using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;

namespace VendersCloud.Business.Service.Concrete
{
    public class RequirementService:IRequirementService
    {
        private readonly IRequirementRepository _requirementRepository;
        public RequirementService(IRequirementRepository requirementRepository)
        {
            _requirementRepository = requirementRepository;
        }

        public async Task<ActionMessageResponse> RequirmentUpsertAsync(RequirementRequest request)
        {
            try
            {
                if (request == null)
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                var response = await _requirementRepository.RequirementUpsertAsync(request);
                if(response !=null)
                {
                    var res = Convert.ToInt64(response);
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Submitted Successfully!! ", Content = response };
                }
                return new ActionMessageResponse() { Success = false, Message = "Requirement Not Submitted  ", Content = "" };

            }
            catch (Exception ex) {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<ActionMessageResponse> DeleteRequirementAsync(int requirementId, string orgCode)
        {
            try
            {
                if((requirementId <=0)||(string.IsNullOrEmpty(orgCode)))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                bool response = await _requirementRepository.DeleteRequirementAsync(requirementId, orgCode);
                if (response)
                {
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Deleted Successfully!! ", Content = "" };
                }
                return new ActionMessageResponse() { Success = false, Message = "Requirement Not Deleted!!  ", Content = "" };
            }
            catch (Exception ex) {
                return new ActionMessageResponse() { Success = false, Message = ex.Message, Content = "" };
            }
        }

        public async Task<List<Requirement>> GetRequirementListAsync()
        {
            try
            {
                var response = await _requirementRepository.GetRequirementListAsync();
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Requirement>> GetRequirementListByIdAsync(int requirementId)
        {
            try
            {
                if(requirementId <=0)
                {
                    return new List<Requirement>();
                }
                var response = await _requirementRepository.GetRequirementListByIdAsync(requirementId);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionMessageResponse> UpdateStatusByIdAsync(int requirementId, int status)
        {
            try
            {
                if(requirementId<=0 || status <=0)
                {
                    return new ActionMessageResponse { Success = false, Message = "Enter Valid Input!!", Content = "" };
                }
                var res = await _requirementRepository.UpdateStatusByIdAsync(requirementId, status);
                if(res)
                    return new ActionMessageResponse { Success = true, Message = "Status Updated Successfully!!", Content = "" };
                return new ActionMessageResponse { Success = false, Message = "Status Not Updated", Content = "" };
            }
            catch(Exception ex)
            {
                return new ActionMessageResponse { Success = false, Message = ex.Message, Content = "" };
            }
        }
    }
}
