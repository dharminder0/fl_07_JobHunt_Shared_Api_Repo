using Microsoft.AspNetCore.Mvc;
using VendersCloud.Business.Entities.RequestModels;
using VendersCloud.Business.Service.Abstract;

namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class RequirementController : BaseApiController
    {
        private readonly IRequirementService _requirementService;
        public RequirementController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Requirement/Upsert")]
        public async Task<IActionResult> RequirmentUpsertAsync([FromBody] RequirementRequest request)
        {
            try
            {
                var result= await _requirementService.RequirmentUpsertAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
