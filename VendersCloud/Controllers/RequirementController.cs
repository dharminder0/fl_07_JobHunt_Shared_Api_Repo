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
        [Route("api/V1/Requirement/GetList/{requirementId}/{orgCode}")]
        public async Task<IActionResult> GetRequirementListByIdAsync(string requirementId,string orgCode)
        {
            try
            {
                var result = await _requirementService.GetRequirementListByIdAsync(requirementId,orgCode);
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
        [HttpPost]
        [Route("api/V1/Requirement/Get/Applicants")]
        public async Task<IActionResult> GetApplicantsListByRequirementIdAsync(GetApplicantsByRequirementRequest request)
        {
            try
            {
                var result = await _requirementService.GetApplicantsListByRequirementIdAsync(request);
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
        public async Task<IActionResult> GetVendorsCountsAsync(string orgCode, string userId, int roleType)
        {
            try
            {
                var result = await _requirementService.GetVendorsCountsAsync(orgCode, userId, roleType);
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
                var result = await _requirementService.GetDayWeekCountsAsyncV2(request);
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
        [HttpPost]
        [Route("api/V1/Requirement/Hot-Vacancies")]
        public async Task<IActionResult> GetHotRequirementAsync(GetHotRequirmentRequest request)
        {
            try
            {
                var result = await _requirementService.GetHotRequirementAsync(request);
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
        [Route("api/V1/Requirement/MatchResult")]
        public async Task<IActionResult> GetRequirementMatchResultAsync([FromBody] RequirementMatchRequest request)
        {
            try
            {
                var result = await _requirementService.GetRequirementMatchResultAsync(request);
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
        [Route("api/V1/Requirement/MatchingVendors")]
        public async Task<IActionResult> GetMatchingVendorsAsync(MatchingVendorRequest request)
        {
            try
            {
                var result = await _requirementService.GetMatchingVendorsAsync(request);
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
        [Route("api/V1/Requirement/SimilerRequirements")]
        public async Task<IActionResult> GetSimilerRequirementsAsync(SimilerRequirmentequest request)
        {
            try
            {
                var result = await _requirementService.GetSimilerRequirementsAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("api/V1/Resources/shared-contracts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        public async Task<IActionResult> GetSharedContracts([FromBody] SharedContractsRequest request)
        {
            var result = await _requirementService.GetSharedContractsAsync(request);
            return Json(result);
        }
        [HttpPost("api/V1/Requirement/matching")]
        public async Task<IActionResult> GetMatchingRequirements([FromBody] MatchingRequirementRequest req)
        {
            if (req == null || req.RequirementId <= 0)
                return BadRequest("Invalid input");

            var result = await _requirementService.GetMatchingRequirementsAsync(req);
            return  Json(result);
        }
    }
}



