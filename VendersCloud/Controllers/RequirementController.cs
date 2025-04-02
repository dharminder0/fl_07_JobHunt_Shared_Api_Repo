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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/Requirement/Get/Applicants")]
        public async Task<IActionResult> GetApplicantsListByRequirementIdAsync(string requirementUniqueId)
        {
            try
            {
                var result = await _requirementService.GetApplicantsListByRequirementIdAsync(requirementUniqueId);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/Requirement/Company-dashboard/{orgCode}")]
        public async Task<IActionResult> GetCountsAsync(string orgCode)
        {
            try
            {
                var result = await _requirementService.GetCountsAsync(orgCode);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/Requirement/Vendor-dashboard/{userId}/{orgCode}")]
        public async Task<IActionResult> GetVendorsCountsAsync(string orgCode,string userId)
        {
            try
            {
                var result = await _requirementService.GetVendorsCountsAsync(orgCode,userId);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Requirement/Company-dashboard/Day-Week/Graph")]
        public async Task<IActionResult> GetDayWeekCountsAsync(CompanyGraphRequest request)
        {
            try
            {
                var result = await _requirementService.GetDayWeekCountsAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Requirement/Company-dashboard/Requirement/Graph")]
        public async Task<IActionResult> GetRequirementCountsAsync(CompanyGraphRequest request)
        {
            try
            {
                var result = await _requirementService.GetRequirementCountsAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Requirement/Vendor-dashboard/Requirement/Graph")]
        public async Task<IActionResult> GetVendorRequirementCountsAsync(VendorGraphRequest request)
        {
            try
            {
                var result = await _requirementService.GetVendorRequirementCountsAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Requirement/Vendor-dashboard/Day-Week/Graph")]
        public async Task<IActionResult> GetVendorDayWeekCountsAsync(VendorGraphRequest request)
        {
            try
            {
                var result = await _requirementService.GetVendorDayWeekCountsAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Requirement/Hot-Upsert")]
        public async Task<IActionResult> HotRequirementUpsertAsync([FromBody] HotRequirementRequest request)
        {
            try
            {
                var result = await _requirementService.HotRequirementUpsertAsync(request);
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
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/V1/Requirement/Hot-Vacancies")]
        public async Task<IActionResult>GetHotRequirementAsync(string orgcode)
        {
            try
            {
                var result = await _requirementService.GetHotRequirementAsync(orgcode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}



