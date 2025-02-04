﻿using VendersCloud.Business.Entities.DataModels;
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
                bool response = await _requirementRepository.RequirementUpsertAsync(request);
                if(response)
                {
                    return new ActionMessageResponse() { Success = true, Message = "Requirement Submitted Successfully!! ", Content = "" };
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
    }
}
