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
                var result = await _requirementService.RequirmentUpsertAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Requirement/Delete")]
        public async Task<IActionResult> DeleteRequirmentAsync(int requirementId, string orgCode)
        {
            try
            {
                var result = await _requirementService.DeleteRequirementAsync(requirementId, orgCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/V1/Requirement/GetList")]
        public async Task<IActionResult> GetRequirementListAsync()
        {
            try
            {
                var result = await _requirementService.GetRequirementListAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/V1/Requirement/GetList/{requirementId}")]
        public async Task<IActionResult> GetRequirementListByIdAsync(string requirementId)
        {
            try
            {
                var result = await _requirementService.GetRequirementListByIdAsync(requirementId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Requirement/UpdateStatus")]
        public async Task<IActionResult> UpdateStatusByIdAsync(int requirementId, int status)
        {
            try
            {
                var result = await _requirementService.UpdateStatusByIdAsync(requirementId, status);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        [Route("api/V1/Requirement/OrgCode")]
        public async Task<IActionResult> GetRequirementByOrgCodeAsync(string orgCode)
        {
            try
            {
                var result = await _requirementService.GetRequirementByOrgCodeAsync(orgCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Requirement/Search")]
        public async Task<IActionResult> SearchRequirementAsync(SearchRequirementRequest request)
        {
            try
            {
                var result = await _requirementService.SearchRequirementAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/V1/Requirement/Count/Total/Applicants")]
        public async Task<IActionResult> GetTotalApplicantsAsync(TotalApplicantsRequest request)
        {
            try
            {
                var result = await _requirementService.GetTotalApplicantsAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}



