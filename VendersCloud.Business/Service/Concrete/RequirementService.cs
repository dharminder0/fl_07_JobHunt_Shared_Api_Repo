using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Entities.ResponseModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class RequirementService:IRequirementService
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public RequirementService(IRequirementRepository requirementRepository,IOrganizationRepository organizationRepository)
        {
            _requirementRepository = requirementRepository;
            _organizationRepository = organizationRepository;
        }

        public async Task<ActionMessageResponse> RequirmentUpsertAsync(RequirementRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Title)|| string.IsNullOrEmpty(request.OrgCode))
                {
                    return new ActionMessageResponse() { Success = false, Message = "Values cann't be null ", Content = "" };
                }
                var uniqueId = Guid.NewGuid().ToString().Substring(0, 12);
                var response = await _requirementRepository.RequirementUpsertAsync(request,uniqueId);
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

        public async Task<List<RequirementResponse>> GetRequirementListAsync()
        {
            try
            {
                var res = new List<RequirementResponse>();
                var response = await _requirementRepository.GetRequirementListAsync();
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var requirementResponse = new RequirementResponse
                        {
                            Id = item.Id,
                            Title = item.Title,
                            OrgCode = item.OrgCode,
                            Description = item.Description,
                            Experience = item.Experience,
                            Budget = item.Budget,
                            Positions = item.Positions,
                            Duration = item.Duration,
                            LocationType = item.LocationType,
                            LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType),
                            Location = item.Location,
                            ClientId = item.ClientId,
                            Remarks = item.Remarks,
                            Visibility = item.Visibility,
                            VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility),
                            Hot = item.Hot,
                            Status = item.Status,
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.UpdatedBy,
                            IsDeleted = item.IsDeleted,
                            Client = new Client() // Ensure the Client object is instantiated
                        };

                        var orgData = await _organizationRepository.GetOrganizationDataByIdAsync(item.ClientId);
                        if (orgData != null)
                        {
                            requirementResponse.Client.ClientName = orgData.OrgName;
                            requirementResponse.Client.ClientLogo = orgData.Logo;
                        }

                        res.Add(requirementResponse);
                    }
                }

                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RequirementResponse> GetRequirementListByIdAsync(string requirementId)
        {
            try
            {
                if (string.IsNullOrEmpty(requirementId))
                {
                    return new RequirementResponse();
                }

                RequirementResponse res = new RequirementResponse
                {
                    Client = new Client() // Ensure the Client object is instantiated
                };

                var response = await _requirementRepository.GetRequirementListByIdAsync(requirementId);

                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var orgData = await _organizationRepository.GetOrganizationDataByIdAsync(item.ClientId);
                        if (orgData != null)
                        {
                            res.Client.ClientName = orgData.OrgName;
                            res.Client.ClientLogo = orgData.Logo;
                        }

                        res.Id = item.Id;
                        res.Title = item.Title;
                        res.OrgCode = item.OrgCode;
                        res.Description = item.Description;
                        res.Experience = item.Experience;
                        res.Budget = item.Budget;
                        res.Positions = item.Positions;
                        res.Duration = item.Duration;
                        res.LocationType = item.LocationType;
                        res.LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType);
                        res.Location = item.Location;
                        res.ClientId = item.ClientId;
                        res.Remarks = item.Remarks;
                        res.Visibility = item.Visibility;
                        res.VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility);
                        res.Hot = item.Hot;
                        res.Status = item.Status;
                        res.CreatedOn = item.CreatedOn;
                        res.UpdatedOn = item.UpdatedOn;
                        res.CreatedBy = item.CreatedBy;
                        res.UpdatedBy = item.UpdatedBy;
                        res.IsDeleted = item.IsDeleted;
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                // Add more context to the exception
                throw new Exception("An error occurred while fetching the requirement list.", ex);
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

        public async Task<List<RequirementResponse>> GetRequirementByOrgCodeAsync(string orgCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orgCode))
                {
                    return new List<RequirementResponse>();
                }

                var res = new List<RequirementResponse>();
                var response = await _requirementRepository.GetRequirementByOrgCodeAsync(orgCode);

                if (response != null)
                {
                    foreach (var item in response)
                    {
                        var requirementResponse = new RequirementResponse
                        {
                            Id = item.Id,
                            Title = item.Title,
                            OrgCode = item.OrgCode,
                            Description = item.Description,
                            Experience = item.Experience,
                            Budget = item.Budget,
                            Positions = item.Positions,
                            Duration = item.Duration,
                            LocationType = item.LocationType,
                            LocationTypeName = Enum.GetName(typeof(LocationType), item.LocationType),
                            Location = item.Location,
                            ClientId = item.ClientId,
                            Remarks = item.Remarks,
                            Visibility = item.Visibility,
                            VisibilityName = Enum.GetName(typeof(Visibility), item.Visibility),
                            Hot = item.Hot,
                            Status = item.Status,
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn,
                            CreatedBy = item.CreatedBy,
                            UpdatedBy = item.UpdatedBy,
                            IsDeleted = item.IsDeleted,
                            Client = new Client() // Ensure the Client object is instantiated
                        };

                        var orgData = await _organizationRepository.GetOrganizationDataByIdAsync(item.ClientId);
                        if (orgData != null)
                        {
                            requirementResponse.Client.ClientName = orgData.OrgName;
                            requirementResponse.Client.ClientLogo = orgData.Logo;
                        }

                        res.Add(requirementResponse);
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                // Consider logging the exception or adding context here
                throw new Exception("An error occurred while fetching requirements.", ex);
            }
        }


    }
}
