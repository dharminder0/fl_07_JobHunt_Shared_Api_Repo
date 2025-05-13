namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class BenchController : BaseApiController
    {
        private readonly IBenchService _benchService;
        public BenchController(IBenchService benchService)
        {
            _benchService = benchService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/Bench/Upsert")]

        public async Task<IActionResult> UpsertBenchAsync(BenchRequest request)
        {
            try
            {
                var result = await _benchService.UpsertBenchAsync(request);
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
        [Route("api/V1/Bench/{orgCode}")]

        public async Task<IActionResult> GetBenchListAsync(string orgCode)
        {
            try
            {
                var result = await _benchService.GetBenchListAsync(orgCode);
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
        [Route("api/V1/Bench/Search")]

        public async Task<IActionResult> GetBenchListBySearchAsync(BenchSearchRequest request)
        {
            try
            {
                var result = await _benchService.GetBenchListBySearchAsync(request);
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
        [Route("api/V1/Bench/GetCv")]
        public async Task<IActionResult> GetCvByIdAsync(int id)
        {
            try
            {
                var result = await _benchService.GetCvByIdAsync(id);
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
        [Route("api/V1/Bench/Cv/UpsertAvtar")]
        public async Task<IActionResult> UpsertCvAvtarAsync(UpsertCvAvtarRequest request)
        {
            try
            {
                var result = await _benchService.UpsertCvAvtarAsync(request);
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
        [Route("api/V1/Bench/Cv/GetAvtar")]
        public async Task<IActionResult> GetAvtarByIdAsync(int benchId)
        {
            try
            {
                var result = await _benchService.GetAvtarByIdAsync(benchId);
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
        [Route("api/V1/Bench/MatchResult")]
        public async Task<IActionResult> GetBenchMatchResultAsync(BenchMatchRecord request)
        {
            try
            {
                var result = await _benchService.GetBenchMatchResultAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/v1/applicant/upsert-status")]
        public async Task<IActionResult> UpsertApplicantStatusHistory([FromBody] ApplicantStatusHistory model)
        {
            try
            {
                if (model == null || model.ApplicantId <= 0 || model.Status <= 0)
                    return BadRequest("Invalid request data.");

                var result = await _benchService.UpsertApplicantStatusHistory(model);
                if (!result)
                    return BadRequest("Failed to insert status history.");

                return Ok("Status history saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpGet]
        [Route("api/v1/applicant/status-history")]
        public async Task<IActionResult> GetApplicantStatusHistory(int applicantId)
        {
            try
            {
                if (applicantId <= 0)
                    return BadRequest("Invalid applicant ID.");

                var result = await _benchService.GetApplicantStatusHistory(applicantId);
                if (result == null || result.Count == 0)
                    return  Json(result);

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}