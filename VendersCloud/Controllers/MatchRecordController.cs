namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class MatchRecordController : BaseApiController
    {
        private readonly IMatchRecordService _matchRecordService;
        public MatchRecordController(IMatchRecordService matchRecordService)
        {
            _matchRecordService = matchRecordService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(RequireAuthorizationFilter))]
        [HttpPost]
        [Route("api/V1/MatchRecord/GetMatchRecord")]
        public async Task<IActionResult> GetMatchRecordAsync([FromBody] MatchRecordRequest request)
        {
            try
            {
                var result = await _matchRecordService.GetMatchRecordAsync(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
