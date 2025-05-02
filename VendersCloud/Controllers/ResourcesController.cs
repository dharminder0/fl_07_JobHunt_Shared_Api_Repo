namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class ResourcesController : BaseApiController
    {
        private readonly IBenchService _benchService;
        private readonly IRequirementService _requirementService;
        public ResourcesController(IBenchService benchService, IRequirementService requirementService)
        {
            _benchService = benchService;
            _requirementService = requirementService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Applications/Upsert")]

        public async Task<IActionResult> UpsertApplicants(ApplicationsRequest request)
        {
            try
            {
                var result = await _benchService.UpsertApplicants(request);
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
        [Route("api/V1/Applications/Search")]

        public async Task<IActionResult> GetSearchApplicantsList(ApplicantsSearchRequest request)
        {
            try
            {
                var result = await _benchService.GetSearchApplicantsList(request);
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
        [Route("api/V1/Applicants/Company/GetListByOrgCode")]
        public async Task<IActionResult> GetRequirementListByOrgCode(CompanyRequirementSearchRequest request)
        {
            try
            {
                var result = await _requirementService.GetRequirementListByOrgCode(request);
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
        [Route("api/V1/Applicants/Company/Vacancies/Active")]
        public async Task<IActionResult> GetActiveVacanciesByOrgCodeAsync(CompanyActiveClientResponse request)
        {
            try
            {
                var result = await _benchService.GetActiveVacanciesByOrgCodeAsync(request);
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
        [Route("api/V1/Applicants/Company/TopVendors")]
        public async Task<IActionResult> GetTopVendorsListAsync(CompanyActiveClientResponse request)
        {
            try
            {
                var result = await _benchService.GetTopVendorsListAsync(request);
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
        [Route("api/V1/Applicants/Vendor/Vacancies/Active")]
        public async Task<IActionResult> GetActiveVacanciesByUserIdAsync(VendorActiveClientResponse request)
        {
            try
            {
                var result = await _benchService.GetActiveVacanciesByUserIdAsync(request);
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
        [Route("api/V1/Resources/TechStack")]
        public async Task<IActionResult> GetCountTechStackByOrgCodeAsync(string orgCode)
        {
            try
            {
                var result = await _benchService.GetCountTechStackByOrgCodeAsync(orgCode);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("api/V1/Resources/contracts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        public async Task<IActionResult> GetVendorContracts([FromBody] VendorContractRequest request)
        {
            var result = await _benchService.GetVendorContractsAsync(request);
            return Ok(result);
        }
    }

}
