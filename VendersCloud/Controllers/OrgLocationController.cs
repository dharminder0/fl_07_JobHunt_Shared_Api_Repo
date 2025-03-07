namespace VendersCloud.WebApi.Controllers
{
    [ApiController]
    public class OrgLocationController : BaseApiController
    {
        private readonly IOrgLocationService _orgLocationService;
        public OrgLocationController(IOrgLocationService orgLocationService)
        {
            _orgLocationService = orgLocationService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        [Route("api/v1/orgLocation/upsert")]
        public async Task<IActionResult> UpsertLocation(OrgLocation location)
        {
            try
            {
                var result = await _orgLocationService.UpsertLocation(location);
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
        [Route("api/v1/orgLocation/get")]
        public async Task<IActionResult> GetOrgLocation(string orgCode)
        {
            try
            {
                var result= await _orgLocationService.GetOrgLocation(orgCode);
                return Json(result);
            }
            catch (Exception ex) {
            return BadRequest(ex.Message);
            }
        }

    }
}
